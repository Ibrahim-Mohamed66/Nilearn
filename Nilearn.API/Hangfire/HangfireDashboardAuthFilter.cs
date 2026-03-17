using Hangfire.Dashboard;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace Nilearn.API.Hangfire
{
    // Secures Hangfire Dashboard for Admins only
    public class HangfireDashboardAuthFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            var httpContext = context.GetHttpContext();

            // Only allow authenticated users
            if (httpContext.User.Identity is not { IsAuthenticated: true })
                return false;

            // Only allow users with "Admin" role
            return httpContext.User.Claims.Any(c => c.Type == "role" && (c.Value == "Admin" || c.Value =="SuperAdmin"));
        }
    }
}