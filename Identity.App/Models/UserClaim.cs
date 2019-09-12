using System;
using Identity.App.Extention;
using Microsoft.AspNetCore.Identity;

namespace Identity.App.Models
{
    public class UserClaim : IdentityUserClaim<Guid>, IAuditableEntity
    {
        public virtual User User { get; set; }
    }
}