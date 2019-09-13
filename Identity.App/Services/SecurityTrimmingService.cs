
using Identity.App.Extention;
using Identity.App.Services.Interface;
using Identity.App.ViewModel;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Security.Claims;
using DNTCommon.Web.Core;


namespace Identity.App.Services
{
    public class SecurityTrimmingService : ISecurityTrimmingService
    {
        private readonly HttpContext _httpContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SecurityTrimmingService(IHttpContextAccessor httpContextAccessor,
            IMvcActionsDiscoveryService mvcActionsDiscoveryService)
        {
            _httpContextAccessor = httpContextAccessor;
            _httpContextAccessor.CheckArgumentIsNull(nameof(_httpContextAccessor));

            _httpContext = _httpContextAccessor.HttpContext;
        }

        public bool CanCurrentUserAccess(string area, string controller, string action)
        {
            return _httpContext != null && CanUserAccess(_httpContext.User, area, controller, action);
        }

        public bool CanUserAccess(ClaimsPrincipal user, string area, string controller, string action)
        {
            var currentClaimValue = $"{area}:{controller}-Controller:{action}-Action";

            if (!user.Identity.IsAuthenticated)
            {
                return false;
            }

            var role = user.Claims.Where(x => x.Type == GlobalEnum.DynamicRole).FirstOrDefault();
            if(role != null && !string.IsNullOrEmpty(role.Value))
                return role.Value.Contains(currentClaimValue);
            
            return false;
        }
    }
}
