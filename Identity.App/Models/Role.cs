using System;
using System.Collections.Generic;
using Identity.App.Extention;
using Microsoft.AspNetCore.Identity;

namespace Identity.App.Models
{
    public class Role : IdentityRole<int>, IAuditableEntity
    {
        public Role() { }

        public Role(string name) : this()
        {
            Name = name;
        }

        public Role(string name, string description) : this(name)
        {
            Description = description;
        }

        public Guid Guid { get; set; }
        public string Title { get; set; }

        public string Description { get; set; }

        public string ActionArray { get; set; }

        public bool Enable { get; set; }

        public virtual ICollection<UserRole> Users { get; set; }

        public virtual ICollection<RoleClaim> Claims { get; set; }
    }
}