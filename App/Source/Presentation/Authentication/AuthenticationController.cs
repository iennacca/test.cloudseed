using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

using MediatR;

namespace CloudSeedApp
{
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        CloudSeedAppDatabaseContext _dbContext;
        ConfigurationProvider _configurationProvider;
        ICurrentUserService _currentUserService;
        IEmailProvider _emailProvider;
        ILogger<AuthenticationController> _logger;
        private readonly IMediator _mediator;
        JwtAuthenticationProcessor _jwtAuthenticationProcessor;
        UserDataProvider _userDataProvider;
        UserManager<User> _userManager;

        public AuthenticationController(
            CloudSeedAppDatabaseContext dbContext,
            ConfigurationProvider configuration,
            ICurrentUserService currentUserService,
            IEmailProvider emailProvider,
            ILogger<AuthenticationController> logger,
            IMediator mediator,
            JwtAuthenticationProcessor jwtAuthenticationProcessor,
            UserDataProvider userDataProvider,
            UserManager<User> userManager
        ) {
            this._dbContext = dbContext;
            this._configurationProvider = configuration;
            this._currentUserService = currentUserService;
            this._emailProvider = emailProvider;
            this._logger = logger;
            this._mediator = mediator;
            this._jwtAuthenticationProcessor = jwtAuthenticationProcessor;
            this._userDataProvider = userDataProvider;
            this._userManager = userManager;
        }

        [HttpGet]
        [Route("/login")]
        [AllowAnonymous]
        public IActionResult Login() {
            var webBaseUrl = this._configurationProvider.WEB_BASE_URL;
            return Redirect(
                $"{webBaseUrl}/login"
            );
        }

        [HttpPost]
        [Route("/login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginApiModel model) {
            var userMaybe = await this._userDataProvider
                .TryGetUserByEmailAsync(model.EmailAddress);
            if(userMaybe is null) {
                throw new ArgumentException(
                    PublicErrorResponseMessages.UserAccountDoesNotExist.ToString()
                );
            } else {
                this._logger.LogInformation("Trying to login user");
                var token = this._jwtAuthenticationProcessor.GenerateAuthenticationToken(
                    userMaybe,
                    JwtAuthenticationPurposes.INITIAL_LOGIN);

                var baseUrl = this._configurationProvider
                    .WEB_BASE_URL + "/login/validate";
                var queryParams = new Dictionary<string, string>() {
                    { "token", token }
                };
                var fullurl = new Uri(
                    QueryHelpers.AddQueryString(
                        baseUrl,
                        queryParams
                    )
                );

                if(fullurl is null) {
                    throw new InvalidOperationException(
                        PublicErrorResponseMessages.Login_FailedToLogIn.ToString()
                    );
                }

                if(this._configurationProvider.IsDevelopmentEnvironment) {
                    this._logger.LogInformation($"Login URL: {fullurl}");
                }
                await this._emailProvider.TrySendEmailAsync(
                    userMaybe.Email,
                    this._configurationProvider.EMAIL_FROM_ADDRESS,
                    "Your Login link",
                    $"Login at: {fullurl}"
                );
            }
            return Ok();
        }

        [HttpGet]
        [Route("/login/session/create")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthenticationSessionDto>> CreateInitialLoginSessionToken(string token) {
            this._logger.LogInformation($"Received LoginValidation attempt: {token}");

            var isValid = this._jwtAuthenticationProcessor
                .TryValidateAuthenticationToken(
                    token,
                    JwtAuthenticationPurposes.INITIAL_LOGIN,
                    out var validatedJwtToken
                );
            if(!isValid) {
                throw new AccessViolationException(
                    PublicErrorResponseMessages.Login_FailedToLogIn.ToString()
                );
            }

            var userId = validatedJwtToken 
                ?.Subject;
            if(userId is null) {
                throw new ArgumentNullException(
                    PublicErrorResponseMessages.Login_FailedToLogIn.ToString()
                );
            };

            var userMaybe = await this._userDataProvider
                .TryGetUserAsync(Guid.Parse(userId));
            if(userMaybe is null) {
                throw new InvalidOperationException(
                    PublicErrorResponseMessages.Login_FailedToLogIn.ToString()
                );
            }

            var newAuthenticationToken = this._jwtAuthenticationProcessor
                .GenerateAuthenticationToken(
                    userMaybe,
                    JwtAuthenticationPurposes.ACCESS
                );

            return Ok(
                new AuthenticationSessionDto {
                    UserId = userMaybe.Id.ToString(),
                    AccessToken = newAuthenticationToken
                }
            );
            // throw new NotImplementedException();
        }

        [HttpGet]
        [Route("/login/session/validate")]
        [Authorize]
        public async Task<ActionResult<AuthenticationSessionDto>> ValidateExistingLoginSession(string token) {
            var isValid = this._jwtAuthenticationProcessor
                .TryValidateAuthenticationToken(
                    token,
                    JwtAuthenticationPurposes.ACCESS,
                    out var validatedJwtToken
                );
            if(!isValid) {
                throw new AccessViolationException(
                    PublicErrorResponseMessages.Login_InvalidAccesstoken.ToString()
                );
            }

            var jwtUserId = validatedJwtToken 
                ?.Subject;
            if(jwtUserId is null) {
                throw new ArgumentNullException(
                    PublicErrorResponseMessages.Login_InvalidAccesstoken.ToString()
                );
            };

            var authorizedUserId = this._currentUserService.UserId;
            if(jwtUserId != authorizedUserId) {
                throw new ArgumentNullException(
                    PublicErrorResponseMessages.Login_InvalidAccesstoken.ToString()
                );
            }

            var getUserQuery = new GetUserQuery { UserId = Guid.Parse(authorizedUserId) };
            var user = await this._mediator
                .Send(getUserQuery);
            if(user is null) {
                throw new InvalidOperationException(
                    PublicErrorResponseMessages.Login_InvalidAccesstoken.ToString()
                );
            }

            return Ok(
                new AuthenticationSessionDto {
                    UserId = user.Id.ToString(),
                    AccessToken = token
                }
            );
            // throw new NotImplementedException();
        }
    }

    public class LoginApiModel {
        [JsonPropertyName("email_address")]
        public string EmailAddress { get; set; }
    }

    public class AuthenticationSessionDto {
        [JsonPropertyName("user_id")]
        public string UserId { get; set; }

        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }
    }
}