using System;
using Identity.App.Models;

namespace Identity.App.ViewModel.Email
{
    public class ChangePasswordNotificationViewModel : EmailsBase
    {
        public User User { set; get; }
    }
}
