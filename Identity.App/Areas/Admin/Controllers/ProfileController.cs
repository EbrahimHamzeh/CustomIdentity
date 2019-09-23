using System.ComponentModel;
using Identity.App.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace Identity.App.Areas.Admin.Controllers
{
    [DisplayName("پروفایل")]
    [Area(AreaConstants.AdminArea)]
    public class ProfileController : Controller
    {
        private readonly IApplicationUserManager _userManager;

        public ProfileController(IApplicationUserManager userManager)
        {
            _userManager = userManager;
        }

        // [BreadCrumb(Title = "ایندکس", Order = 1)]
        [DisplayName("پروفایل")]
        public IActionResult Index() => View();

        public IActionResult ChangePassword() {
           return View();
        }
    }
}