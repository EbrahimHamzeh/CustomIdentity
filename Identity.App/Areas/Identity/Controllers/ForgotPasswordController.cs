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
using Identity.App.ViewModel.Email;
using DNTPersianUtils.Core;

namespace Identity.App.Areas.Identity.Controllers
{
    [Area(AreaConstants.IdentityArea)]
    [AllowAnonymous]
    public class ForgotPasswordController : Controller
    {
        private readonly IEmailSender _emailSender;
        private readonly ILogger<LoginController> _logger;
        private readonly IApplicationSignInManager _signInManager;
        private readonly IApplicationUserManager _userManager;
        private readonly IOptionsSnapshot<AppSettings> _appSettings;

        public ForgotPasswordController(
            ILogger<LoginController> logger,
            IApplicationSignInManager signInManager,
            IApplicationUserManager userManager,
            IEmailSender emailSender,
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
            
            _emailSender = emailSender;
            _emailSender.CheckArgumentIsNull(nameof(_emailSender));
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
        public async Task<IActionResult> Index(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user == null)
                {
                    ModelState.AddModelError("Email","چنین کاربری در سیستم موجود نمی باشد.");
                    return View(model);
                }

                var code = await _userManager.GeneratePasswordResetTokenAsync(user);

                await _emailSender.SendEmailAsync(
                   email: model.Email,
                   subject: "بازیابی کلمه‌ی عبور",
                   viewNameOrPath: "~/Areas/Identity/Views/EmailTemplates/_PasswordReset.cshtml",
                   model: new PasswordResetViewModel
                   {
                       UserId = user.Id,
                       Token = code,
                       EmailSignature = _appSettings.Value.Smtp.FromName,
                       MessageDateTime = DateTime.UtcNow.ToLongPersianDateTimeString()
                   });
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