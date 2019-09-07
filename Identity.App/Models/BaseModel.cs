using System;
using Identity.App.Extention;

namespace Identity.App.Models
{
    public class BaseModel : IAuditableEntity
    {
        public Guid Guid { get; set; }

    }
}