using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Identity.App.Services.Interface;
using Identity.App.Extention;
using DNTCommon.Web.Core;
using System;
using System.Threading.Tasks;
using Identity.App.ViewModel;
using Microsoft.Extensions.Options;
using Identity.App.Settings;
using DNTCaptcha.Core;

namespace Identity.App.Areas.Identity.Controllers
{
    [Area(AreaConstants.IdentityArea)]
    [AllowAnonymous]
    public class LoginController : Controller
    {
        private readonly ILogger<LoginController> _logger;
        private readonly IApplicationSignInManager _signInManager;
        private readonly IApplicationUserManager _userManager;
        private readonly IOptionsSnapshot<AppSettings> _appSettings;

        public LoginController(
            ILogger<LoginController> logger,
            IApplicationSignInManager signInManager,
            IApplicationUserManager userManager,
            IOptionsSnapshot<AppSettings> appSettings)
        {
            _logger = logger;
            _logger.CheckArgumentIsNull(nameof(_logger));

            _signInManager = signInManager;
            _signInManager.CheckArgumentIsNull(nameof(_signInManager));

            _userManager = userManager;
            _userManager.CheckArgumentIsNull(nameof(_userManager));

            _appSettings = appSettings;
            _appSettings.CheckArgumentIsNull(nameof(_appSettings));
        }

        [NoBrowserCache]
        public IActionResult Index(string returnUrl = null)
        {
            ViewData["returnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateDNTCaptcha(CaptchaGeneratorLanguage = DNTCaptcha.Core.Providers.Language.Persian)]
        public async Task<IActionResult> Index(LoginViewModel model, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(model.Username);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "نام کاربری و یا کلمه عبور وارد شده معتبر نمی باشد.");
                    return View(model);
                }

                if (!user.IsActive)
                {
                    ModelState.AddModelError(string.Empty, "اکانت شما غیرفعال شده‌است.");
                    return View(model);
                }

                if (_appSettings.Value.EnableEmailConfirmation && !await _userManager.IsEmailConfirmedAsync(user))
                {
                    ModelState.AddModelError(string.Empty, "لطفا به پست الکترونیک خود مراجعه کرده و ایمیل خود را تائید کنید!");
                    return View(model);
                }

                var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberMe, lockoutOnFailure: true);

                if (result.Succeeded)
                {
                    _logger.LogInformation(1, $"{model.Username} logged in.");
                    if(Url.IsLocalUrl(returnUrl))
                    {
                        Redirect(returnUrl);
                    }
                    return RedirectToAction(nameof(Admin.Controllers.DashboardController.Index), "Dashboard", new { Area="Admin" });
                }

                if (result.RequiresTwoFactor)
                {
                    // TODO: تکمیل نیست
                }

                if(result.IsLockedOut){
                    _logger.LogWarning(2, $"{model.Username} قفل شده است.");
                    return null; // TODO: چنین صفحه ای ایجاد نشده است
                }

                if(result.IsNotAllowed)
                {
                    ModelState.AddModelError(string.Empty, "عدم دسترسی ورود.");
                    return View(model);
                }

                ModelState.AddModelError(string.Empty, "نام کاربری و یا کلمه‌ی عبور وارد شده معتبر نیستند.");
                return View(model);
            }

            return View(model);
        }

        public async Task<IActionResult> LogOff(){
            var user = User.Identity.IsAuthenticated ? await _userManager.FindByNameAsync(User.Identity.Name) : null;
            await _signInManager.SignOutAsync();

            if(user != null)
            {
                await _userManager.UpdateSecurityStampAsync(user);
                _logger.LogInformation(4, $"{user.UserName} logged out.");
            }

            return RedirectToAction(nameof(App.Controllers.HomeController.Index), "Home");
        }
    }
}