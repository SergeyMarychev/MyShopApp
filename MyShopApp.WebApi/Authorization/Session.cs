using Microsoft.AspNetCore.Identity;
using MyShopApp.Application.Authorization;
using MyShopApp.Domain.Users;

namespace MyShopApp.WebApi.Authorization
{
    public class Session : IAppSession
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<User> _userManager;

        public Session(IHttpContextAccessor httpContextAccessor, UserManager<User> userManager)
        {
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }

        public long? UserId => GetUserIdOrDefault();

        private long? GetUserIdOrDefault()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext?.User == null)
            {
                return null;
            }

            var idString = _userManager.GetUserId(httpContext.User);

            if (string.IsNullOrWhiteSpace(idString))
            {
                return null;
            }

            if (long.TryParse(idString, out var userId))
            {
                return userId;
            }

            return null;
        }
    }
}
