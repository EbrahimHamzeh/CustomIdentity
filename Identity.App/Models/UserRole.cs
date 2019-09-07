using System;
using Identity.App.Extention;
using Microsoft.AspNetCore.Identity;

namespace Identity.App.Models
{
    public class UserRole : IdentityUserRole<int>, IAuditableEntity
    {
        public Guid Guid { get; set; }
        public virtual User User { get; set; }

        public virtual Role Role { get; set; }
    }
}