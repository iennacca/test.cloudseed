using Microsoft.AspNetCore.Http;
using System;
using System.Security.Claims;

namespace CloudSeedApp
{
    public class CurrentUserHttpContextProcessor: ICurrentUserService
    {
        public string? UserId => _httpContextAccessor 
            .HttpContext?
            .User?
            .FindFirstValue(ClaimTypes.NameIdentifier);
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserHttpContextProcessor(IHttpContextAccessor httpContextAccessor) {
             _httpContextAccessor = httpContextAccessor;
        }
    }
}