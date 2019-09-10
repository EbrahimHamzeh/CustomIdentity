using System;
using System.Linq;
using System.Threading.Tasks;
using Identity.App.Extention;
using Identity.App.Services.Interface;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Identity.App.TagHelpers
{

    [HtmlTargetElement("secure-content")]
    public class SecureContentTagHelper : TagHelper
    {
        private readonly IApplicationUserManager _userManager;

        public SecureContentTagHelper(IApplicationUserManager userManager)
        {
            _userManager = userManager;
            _userManager.CheckArgumentIsNull(nameof(_userManager));
        }

        [HtmlAttributeName("asp-area")]
        public string Area { get; set; }

        [HtmlAttributeName("asp-controller")]
        public string Controller { get; set; }

        [HtmlAttributeName("asp-action")]
        public string Action { get; set; }

        [ViewContext, HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = null;
            var user = ViewContext.HttpContext.User;

            if (!user.Identity.IsAuthenticated)
            {
                output.SuppressOutput();
                return;
            }

            var actionId = $"{Area}:{Controller}-Controller:{Action}-Action";

            var roles = _userManager.GetCurrentUserAccessInAction();
            if(!string.IsNullOrEmpty(roles)){
                var roleArray = roles.Split(',');
                if(roleArray.Contains(actionId))
                    return;
            }

            output.SuppressOutput();
        }
    }
}
