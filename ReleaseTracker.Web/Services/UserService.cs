using System.Security.Principal;

namespace ReleaseTracker.Web.Services
{
    public interface IUserService
    {
        string GetCurrentUserName();
    }

    public class UserService : IUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetCurrentUserName()
        {
            var httpContext = _httpContextAccessor.HttpContext;

            if (httpContext == null)
                return "System";

            // Try ASP.NET Identity first (if configured)
            var identityName = httpContext.User?.Identity?.Name;
            if (!string.IsNullOrEmpty(identityName))
            {
                // Clean up Windows domain format (DOMAIN\username -> username)
                if (identityName.Contains('\\'))
                {
                    var parts = identityName.Split('\\');
                    return parts[parts.Length - 1];
                }
                return identityName;
            }

            // Fallback to environment variable (OS-agnostic)
            // Works on Windows, Linux, and macOS
            var envUser = Environment.UserName;
            if (!string.IsNullOrEmpty(envUser))
                return envUser;

            // Last resort
            return "System";
        }
    }
}
