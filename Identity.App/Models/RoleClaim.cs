using System;
using Identity.App.Extention;
using Microsoft.AspNetCore.Identity;

namespace Identity.App.Models
{
    public class RoleClaim : IdentityRoleClaim<Guid>, IAuditableEntity
    {
        public virtual Role Role { get; set; }
    }
}