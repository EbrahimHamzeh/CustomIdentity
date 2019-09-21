using System.ComponentModel.DataAnnotations;

namespace Identity.App.ViewModel
{
    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "ایمیا الزامی می‌باشد")]
        [EmailAddress(ErrorMessage = "لطفا آدرس ایمیل معتبر وارد کنید.")]
        public string Email { get; set; }
    }
}
