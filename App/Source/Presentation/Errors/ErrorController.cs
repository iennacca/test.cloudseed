using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using System;

namespace CloudSeedApp
{
    [ApiController]
    public class ErrorController : ControllerBase
    {
        public ErrorController() { }

        // Modeled from https://docs.microsoft.com/en-us/aspnet/core/web-api/handle-errors?view=aspnetcore-6.0#exception-handler
        [Route("/error-development")]
        [AllowAnonymous]
        public IActionResult HandleErrorDevelopment(
            [FromServices] IHostEnvironment hostEnvironment
        ) {
            if(!hostEnvironment.IsDevelopment()) {
                return NotFound();
            }

            var exceptionHandlerFeature = HttpContext.Features.Get<IExceptionHandlerFeature>();

            return Problem(
                detail: exceptionHandlerFeature.Error.StackTrace,
                title: exceptionHandlerFeature.Error.Message
            );
        }

        [Route("/error")]
        [AllowAnonymous]
        private IActionResult HandleError() {
            var exceptionHandlerFeature = HttpContext.Features.Get<IExceptionHandlerFeature>();

            // We don't want to let non-sanitized error messages out to production
            // in case they hold sensitive data
            var errorResponseExists = Enum.TryParse(
                typeof(PublicErrorResponseMessages),
                exceptionHandlerFeature.Error.Message,
                out var parsedEnum);
            
            string sanitizedErrorMessage = errorResponseExists
                ? ((PublicErrorResponseMessages)parsedEnum).ToString()
                : PublicErrorResponseMessages.InternalServerError.ToString();

            return Problem(
                title: sanitizedErrorMessage
            );
        }
    }
}