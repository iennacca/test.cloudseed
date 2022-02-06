using System;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CloudSeedApp
{
    [ApiController]
    public class UserController : ControllerBase
    {
        CloudSeedAppDatabaseContext _dbContext;
        ICurrentUserService _currentUserService;
        ILogger<UserController> _logger;
        IMediator _mediator;
        UserDataProvider _userDataProvider;

        public UserController(
            CloudSeedAppDatabaseContext dbContext,
            ICurrentUserService currentUserService,
            ILogger<UserController> logger,
            IMediator mediator,
            UserDataProvider userDataProvider
        ) {
            this._currentUserService = currentUserService;
            this._dbContext = dbContext;
            this._logger = logger;
            this._mediator = mediator;
            this._userDataProvider = userDataProvider;
        }

        [HttpPost]
        [Route("/users/register")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterUserAsync(RegisterUserPayload registerUserPayload) {
            var isEmailValid = await TryValidateEmailAddress(registerUserPayload.EmailAddress);
            if(!isEmailValid) {
                throw new InvalidOperationException(
                    PublicErrorResponseMessages.InvalidEmailAddress.ToString()
                );
            }

            var userWithEmailAddressMaybe = await this._userDataProvider
                .TryGetUserByEmailAsync(registerUserPayload.EmailAddress);
            if(userWithEmailAddressMaybe is not null) {
                throw new InvalidOperationException(
                    PublicErrorResponseMessages.RegisterUser_UserWithEmailAlreadyExists.ToString()
                );
            }

            this._logger.LogInformation("Creating user");

            var newUser = new User(
                registerUserPayload.EmailAddress,
                new User.UserData {}
            );
            this._dbContext
                .Users
                .Add(newUser);
            await this._dbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpGet]
        [Route("/users/{userId}")]
        [Authorize]
        public async Task<ActionResult<GetUserApiPayload>> GetUser(string userId) {
            var requestUserId = this._currentUserService.UserId;
            if(requestUserId != userId) {
                throw new InvalidOperationException(
                    "User does not have permissions to access this endpoint!"
                );
            }

            var user = await this._userDataProvider
                .TryGetUserAsync(Guid.Parse(userId));
            if(user is null) {
                throw new InvalidOperationException(
                    $"Could not find user for userId: {userId}"
                );
            }

            return Ok(
                new GetUserApiPayload {
                    UserId = user.Id.ToString(),
                    EmailAddress = user.Email,
                    GitHubUsername = user.Data.GitHubUsername
                }
            );
        }

        [HttpPost]
        [Route("/users/{userId}")]
        [Authorize]
        public async Task<IActionResult> UpdateUser(
            string userId,
            PostUserApiPayload payload
        ) {
            var requestUserId = this._currentUserService.UserId;
            if(requestUserId != userId) {
                throw new InvalidOperationException(
                    $"User does not have permissions to access this endpoint!"
                );
            }

            var getUserQuery = new GetUserQuery { UserId = Guid.Parse(userId) };
            var user = await this._mediator.Send(getUserQuery);
            if(user is null) {
                throw new InvalidOperationException(
                    $"Could not find user for userId: {userId}"
                );
            }

            if(payload.EmailAddress is not null) {
                user.Email = payload.EmailAddress;
            }

            if(payload.GitHubUsername is not null) {
                user.Data.GitHubUsername = payload.GitHubUsername;
                this._dbContext.MarkDataAsModified(user);
            }
            await this._dbContext.SaveChangesAsync();

            return Ok();
        }

        private async Task<bool> TryValidateEmailAddress(string emailAddress) {
            return emailAddress is not null 
                && emailAddress.Length > 0
                && emailAddress.Contains("@")
                && emailAddress.Contains(".");
        }

        public class RegisterUserPayload {

            [JsonPropertyName("email_address")]
            public string EmailAddress { get; set; }
        }

        public class GetUserApiPayload {

            [JsonPropertyName("userId")]
            public string UserId { get; set; }

            [JsonPropertyName("emailAddress")]
            public string EmailAddress { get; set; }

            [JsonPropertyName("gitHubUsername")]
            public string GitHubUsername { get; set; }
        }

        public class PostUserApiPayload {
            [JsonPropertyName("emailAddress")]
            public string? EmailAddress { get; set; }

            [JsonPropertyName("gitHubUsername")]
            public string? GitHubUsername { get; set; }
        }
    }
}