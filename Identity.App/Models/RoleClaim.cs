using System;
using Identity.App.Extention;
using Microsoft.AspNetCore.Identity;

namespace Identity.App.Models
{
    public class RoleClaim : IdentityRoleClaim<int>, IAuditableEntity
    {
        public Guid Guid { get; set; }
        public virtual Role Role { get; set; }
    }
}