using System;
using System.ComponentModel.DataAnnotations;

namespace Identity.App.ViewModel
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "نام‌کاربری الزامی است")]
        public string Username { get; set; }
        
        [Required(ErrorMessage = "رمز عبور الزامیست")]
        public string Password { get; set; }
        
        public bool RememberMe { get; set; }
    }
}
