using System;
using Identity.App.Extention;
using Microsoft.AspNetCore.Identity;

namespace Identity.App.Models
{
    public class UserToken : IdentityUserToken<int>, IAuditableEntity
    {
        public Guid Guid { get; set; }
        public virtual User User { get; set; }
    }
}