
using System;
using Identity.App.Extention;

namespace Identity.App.Models
{
    public class UserUsedPassword : BaseModel
    {
        public int Id { get; set; }
        public string HashedPassword { get; set; }
        public int UserId { get; set; }
        public virtual User User { get; set; }
    }
}