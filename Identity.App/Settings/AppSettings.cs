using System;
using DNTCommon.Web.Core;
using Identity.App.Settings.Details;
using Microsoft.AspNetCore.Identity;

namespace Identity.App.Settings
{
    public class AppSettings
    {
        public AdminUserSeed AdminUserSeed { get; set; }
        public bool EnableEmailConfirmation { get; set; }
        public SmtpConfig Smtp { get; set; }
        public string[] EmailsBanList { get; set; }
        public string[] PasswordsBanList { get; set; }
        public CookieOptions CookieOptions { get; set; }
        public LockoutOptions LockoutOptions { get; set; }
        public int NotAllowedPreviouslyUsedPasswords { get; set; }
        public int ChangePasswordReminderDays { get; set; }
        public UserAvatarImageOptions UserAvatarImageOptions { get; set; }
        public PasswordOptions PasswordOptions { get; set; }
    }
}
