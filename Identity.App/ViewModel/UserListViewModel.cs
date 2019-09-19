using System;
using System.ComponentModel.DataAnnotations;

namespace Identity.App.ViewModel
{
    public class UserListViewModel
    {
        public Guid Guid { get; set; }

        public string Username { get; set; }
        
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public bool IsActive { get; set; }

        public DateTimeOffset? LockoutEnd { get; set; }

        public bool EmailConfirmed { get; set; }

        public bool LockoutEnabled { get; set; }

        public bool TwoFactorEnabled { get; set; }

    }
}
