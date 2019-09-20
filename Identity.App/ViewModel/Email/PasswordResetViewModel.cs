using System;

namespace Identity.App.ViewModel.Email
{
    public class PasswordResetViewModel: EmailsBase
    {
        public Guid UserId { get; set; }
        public string Token { get; set; }
    }
}
