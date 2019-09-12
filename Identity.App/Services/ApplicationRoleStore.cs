using System;
using System.Collections.Generic;
using Identity.App.Models;
using Identity.App.Models.Context;
using Identity.App.Services.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Identity.App.Services
{
    public class ApplicationRoleStore :
        RoleStore<Role, AppDbContext, Guid, UserRole, RoleClaim>,
        IApplicationRoleStore
    {
        public ApplicationRoleStore(IUnitOfWork uow,IdentityErrorDescriber describer): base((AppDbContext)uow, describer)
        {
        }
    }
}
