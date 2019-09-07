using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Identity.App.ViewModel
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "نام کاربری الزامی میباشد")]
        [RegularExpression("^[a-zA-Z_]*$", ErrorMessage = "لطفا تنها از حروف انگلیسی استفاده نمائید")]
        [Remote("ValidateUsername", "Register", AdditionalFields = nameof(Email) + "," + "__RequestVerificationToken", HttpMethod = "POST")]
        public string Username { get; set; }

        [Required(ErrorMessage = "نام الزامی است")]
        [StringLength(450, ErrorMessage = "طول نام نمیتواند از 450 بیشتر باشد.")]
        [RegularExpression(@"^[\u0600-\u06FF,\u0590-\u05FF\s]*$", ErrorMessage = "لطفا تنها از حروف فارسی استفاده نمائید")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "نام‌خانوادگی الزامی است")]
        [StringLength(450, ErrorMessage = "طول نام نمیتواند از 450 بیشتر باشد.")]
        [RegularExpression(@"^[\u0600-\u06FF,\u0590-\u05FF\s]*$", ErrorMessage = "لطفا تنها از حروف فارسی استفاده نمائید")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "ایمیا الزامی می‌باشد")]
        [EmailAddress(ErrorMessage = "لطفا آدرس ایمیل معتبر وارد کنید.")]
        [Remote("ValidateUsername", "Register", AdditionalFields = nameof(Username) + "," + "__RequestVerificationToken", HttpMethod = "POST")]
        public string Email { get; set; }

        [Required(ErrorMessage = "رمز عبور الزامی می‌باشد.")]
        [StringLength(100, ErrorMessage = "{0} باید حداقل {2} و حداکثر {1} حرف باشند.", MinimumLength = 6)]
        [Remote("ValidatePassword", "Register",AdditionalFields = nameof(Username) + "," + "__RequestVerificationToken", HttpMethod = "POST")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage="تایید رمز عبور الزامی است")]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "کلمات عبور وارد شده با هم تطابق ندارند")]
        public string ConfirmPassword { get; set; }

    }
}
