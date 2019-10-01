using System;
using System.ComponentModel;
using System.Threading.Tasks;
using DNTPersianUtils.Core;
using Identity.App.Services.Interface;
using Identity.App.Settings;
using Identity.App.ViewModel;
using Identity.App.ViewModel.Email;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Identity.App.Areas.Admin.Controllers
{
    [DisplayName("پروفایل")]
    [Area(AreaConstants.AdminArea)]
    public class ProfileController : Controller
    {
        private readonly IApplicationUserManager _userManager;
        private readonly IApplicationSignInManager _signInManager;
        private readonly IEmailSender _email;
        private readonly IOptionsSnapshot<AppSettings> _appSettings;

        public ProfileController(IApplicationUserManager userManager, IApplicationSignInManager signInManager, IEmailSender email, IOptionsSnapshot<AppSettings> appSettings)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _email = email;
            _appSettings = appSettings;
        }

        // [BreadCrumb(Title = "ایندکس", Order = 1)]
        [DisplayName("پروفایل")]
        public IActionResult Index() => View();

        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var user = await _userManager.GetCurrentUserAsync();
                if (user == null)
                    return View("NotFound");

                var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);

                if (result.Succeeded)
                {
                    await _userManager.UpdateSecurityStampAsync(user);
                    await _signInManager.RefreshSignInAsync(user);

                    await _email.SendEmailAsync(
                           email: user.Email,
                           subject: "اطلاع رسانی تغییر کلمه‌ی عبور",
                           viewNameOrPath: "~/Areas/Identity/Views/EmailTemplates/_ChangePasswordNotification.cshtml",
                           model: new ChangePasswordNotificationViewModel
                           {
                               User = user,
                               EmailSignature = _appSettings.Value.Smtp.FromName,
                               MessageDateTime = DateTime.UtcNow.ToLongPersianDateTimeString()
                           });

                }
            }
            return View(model);
        }
    }
}