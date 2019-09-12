using System;
using Identity.App.Extention;
using Microsoft.AspNetCore.Identity;

namespace Identity.App.Models
{
    public class UserRole : IdentityUserRole<Guid>, IAuditableEntity
    {
        public virtual User User { get; set; }
        public virtual Role Role { get; set; }
    }
}