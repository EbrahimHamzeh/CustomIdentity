using System;
using System.ComponentModel.DataAnnotations;

namespace Identity.App.ViewModel
{
    public class ChangePasswordViewModel
    {
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }
        
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "{0} باید حداقل {2} و حداکثر {1} حرف باشند.", MinimumLength = 6)]
        public string NewPassword { get; set; }
        
        [DataType(DataType.Password)]
        [Compare(nameof(NewPassword), ErrorMessage = "کلمات عبور وارد شده با هم تطابق ندارند")]
        public string ConfirmNewPassword { get; set; }
        public DateTimeOffset? LastUserPasswordChangeDate { get; set; }
    }
}
