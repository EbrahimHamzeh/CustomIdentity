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
using Microsoft.AspNetCore.Identity;
using Identity.App.Models;

namespace Identity.App.Areas.Identity.Controllers
{
    [Area(AreaConstants.IdentityArea)]
    [AllowAnonymous]
    public class RegisterController : Controller
    {
        private readonly IEmailSender _emailSender;
        private readonly ILogger<RegisterController> _logger;
        private readonly IApplicationUserManager _userManager;
        private readonly IPasswordValidator<User> _passwordValidator;
        private readonly IUserValidator<User> _userValidator;
        private readonly IOptionsSnapshot<AppSettings> _appSettings;

        public RegisterController(
            IEmailSender emailSender,
            ILogger<RegisterController> logger,
            IApplicationUserManager userManager,
            IPasswordValidator<User> passwordValidator,
            IUserValidator<User> userValidator,
            IOptionsSnapshot<AppSettings> appSettings)
        {
            _emailSender = emailSender;
            _emailSender.CheckArgumentIsNull(nameof(_emailSender));

            _logger = logger;
            _logger.CheckArgumentIsNull(nameof(_logger));

            _userManager = userManager;
            _userManager.CheckArgumentIsNull(nameof(_userManager));

            _passwordValidator = passwordValidator;
            _passwordValidator.CheckArgumentIsNull(nameof(_passwordValidator));

            _userValidator = userValidator;
            _userValidator.CheckArgumentIsNull(nameof(_userValidator));

            _appSettings = appSettings;
            _appSettings.CheckArgumentIsNull(nameof(_appSettings));

        }

        [AjaxOnly, HttpPost, ValidateAntiForgeryToken]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> ValidateUsername(string username, string email)
        {
            var result = await _userValidator.ValidateAsync(
                (UserManager<User>)_userManager, new User { UserName = username, Email = email }
            );

            return Json(result.Succeeded ? "true" : result.DumpErrors(useHtmlNewLine: true));
        }

        [AjaxOnly, HttpPost, ValidateAntiForgeryToken]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> ValidatePassword(string password, string username)
        {
            var result = await _passwordValidator.ValidateAsync(
                 (UserManager<User>)_userManager, new User { UserName = username }, password
            );

            return Json(result.Succeeded ? "true" : result.DumpErrors(useHtmlNewLine: true));
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateDNTCaptcha(CaptchaGeneratorLanguage = DNTCaptcha.Core.Providers.Language.Persian)]
        public async Task<IActionResult> Index(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new User
                {
                    UserName = model.Username,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation(3, $"{user.UserName} created a new account with password.");

                    if (_appSettings.Value.EnableEmailConfirmation)
                    {
                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                        await _emailSender.SendEmailAsync(
                            email: user.Email,
                            subject: "لطفا اکانت خود را تائید کنید",
                            message: "پیام تست"
                            // viewNameOrPath: "~/Areas/Identity/Views/EmailTemplates/_RegisterEmailConfirmation.cshtml",
                            // model: new RegisterEmailConfirmationViewModel
                            // {
                            //     User = user,
                            //     EmailConfirmationToken = code,
                            //     EmailSignature = _siteOptions.Value.Smtp.FromName,
                            //     MessageDateTime = DateTime.UtcNow.ToLongPersianDateTimeString()
                            // }
                        );
                        
                        //return RedirectToAction(nameof(ConfirmYourEmail));
                    }
                }
            }

            return View(model);
        }
    }
}