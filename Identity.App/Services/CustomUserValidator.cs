using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Identity.App.Extention;
using Identity.App.Models;
using Identity.App.Models.Context;
using Identity.App.Services.Interface;
using Identity.App.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Identity.App.Services
{
    public class CustomUserValidator : UserValidator<User>
    {
        private readonly ISet<string> _emailsBanList;

        public CustomUserValidator(IdentityErrorDescriber errors, IOptionsSnapshot<AppSettings> configurationRoot) : base(errors)
        {
            configurationRoot.CheckArgumentIsNull(nameof(configurationRoot));
            _emailsBanList = new HashSet<string>(configurationRoot.Value.EmailsBanList, StringComparer.OrdinalIgnoreCase);

            if (!_emailsBanList.Any())
            {
                throw new InvalidOperationException("Please fill the emails ban list in the appsettings.json file.");
            }
        }

        public override async Task<IdentityResult> ValidateAsync(UserManager<User> manager, User user)
        {
            var result = await base.ValidateAsync(manager, user);
            var errors = result.Succeeded ? new List<IdentityError>() : result.Errors.ToList();

            validateEmail(user, errors);
            validateUserName(user, errors);

            return !errors.Any() ? IdentityResult.Success : IdentityResult.Failed(errors.ToArray());
        }

        private void validateEmail(User user, List<IdentityError> errors)
        {
            var userEmail = user?.Email;
            if (string.IsNullOrWhiteSpace(userEmail))
            {
                errors.Add(new IdentityError {
                    Code = "EmailIsNotSet",
                    Description = "لطفا اطلاعات ایمیل را تکمیل کنید."
                });
                return;
            }

            if (_emailsBanList.Any(email => userEmail.EndsWith(email, StringComparison.OrdinalIgnoreCase)))
            {
                errors.Add(new IdentityError {
                    Code = "BadEmailDomainError",
                    Description = "لطفا یک ایمیل پروایدر معتبر را وارد نمائید."
                });
            }
        }

        private void validateUserName(User user, List<IdentityError> errors)
        {
            var username = user?.UserName;
            if (string.IsNullOrWhiteSpace(username))
            {
                errors.Add(new IdentityError
                {
                    Code = "IserIsNotSet",
                    Description = "لطفا اطلاعات کاربری را تکمیل کنید."
                });
                return;  // base.ValidateAsync() will cover this case
            }

            if (username.IsNumeric() || username.ContainsNumber())
            {
                errors.Add(new IdentityError
                {
                    Code = "BadUserNameError",
                    Description = "نام کاربری نمی تواند حاوی اعداد باشد."
                });
            }

            if (username.HasConsecutiveChars())
            {
                errors.Add(new IdentityError {
                    Code = "BadUserNameError",
                    Description = "نام کاربری وارد شده معتبر نمی باشد."
                });
            }
        }
    }
}
