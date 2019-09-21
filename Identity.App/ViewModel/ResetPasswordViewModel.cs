using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Identity.App.ViewModel
{
    public class ResetPasswordViewModel
    {
        [Required(ErrorMessage = "ایمیا الزامی می‌باشد")]
        [EmailAddress(ErrorMessage = "لطفا آدرس ایمیل معتبر وارد کنید.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "رمز عبور الزامی می‌باشد.")]
        [StringLength(100, ErrorMessage = "{0} باید حداقل {2} و حداکثر {1} حرف باشند.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage="تایید رمز عبور الزامی است")]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "کلمات عبور وارد شده با هم تطابق ندارند")]
        public string ConfirmPassword { get; set; }

        [FromQuery(Name = "code")]
        public string Code { get; set; }
    }
}
