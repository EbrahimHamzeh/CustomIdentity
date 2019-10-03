using System;
using System.Linq;
using System.Threading.Tasks;
using Identity.App.Defind;
using Identity.App.Extention;
using Identity.App.Models;
using Identity.App.Models.Context;
using Identity.App.Services.Interface;
using Identity.App.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;

namespace Identity.App.Services
{
    public class IdentityDbInitializer : IIdentityDbInitializer
    {
        private readonly IApplicationUserManager _applicationUserManager;
        private readonly ILogger<IdentityDbInitializer> _logger;
        private readonly IApplicationRoleManager _roleManager;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IMvcControllerDiscovery _mvcControllerDiscovery;
        private readonly IHostingEnvironment _env;
        private readonly IUnitOfWork _uow;

        public IdentityDbInitializer(
            IApplicationUserManager applicationUserManager,
            IServiceScopeFactory scopeFactory,
            IApplicationRoleManager roleManager,
            ILogger<IdentityDbInitializer> logger,
            IMvcControllerDiscovery mvcControllerDiscovery,
            IHostingEnvironment env,
            IUnitOfWork uow
            )
        {
            _applicationUserManager = applicationUserManager;
            _applicationUserManager.CheckArgumentIsNull(nameof(_applicationUserManager));

            _scopeFactory = scopeFactory;
            _scopeFactory.CheckArgumentIsNull(nameof(_scopeFactory));

            _roleManager = roleManager;
            _roleManager.CheckArgumentIsNull(nameof(_roleManager));

            _logger = logger;
            _logger.CheckArgumentIsNull(nameof(_logger));

            _mvcControllerDiscovery = mvcControllerDiscovery;
            _env = env;

            _uow = uow;
        }

        /// <summary>
        /// Applies any pending migrations for the context to the database.
        /// Will create the database if it does not already exist.
        /// </summary>
        public void Initialize()
        {
            using (var serviceScope = _scopeFactory.CreateScope())
            {
                using (var context = serviceScope.ServiceProvider.GetService<AppDbContext>())
                {
                    context.Database.Migrate();
                }
            }
        }

        /// <summary>
        /// Adds some default values to the IdentityDb
        /// </summary>
        public void SeedData()
        {
            using (var serviceScope = _scopeFactory.CreateScope())
            {
                var identityDbSeedData = serviceScope.ServiceProvider.GetService<IIdentityDbInitializer>();
                var result = identityDbSeedData.SeedDatabaseWithAdminUserAsync().Result;
                if (result == IdentityResult.Failed())
                {
                    throw new InvalidOperationException(result.DumpErrors());
                }

                // How to add initial data to the DB directly
                using (var context = serviceScope.ServiceProvider.GetService<AppDbContext>())
                {
                    if (!context.Roles.Any())
                    {
                        context.Add(new Role(ConstantRoles.Admin));
                        context.SaveChanges();
                    }
                }
            }
        }

        public async Task<IdentityResult> SeedDatabaseWithAdminUserAsync()
        {
            var name = "admin";
            var password = "Ebr@h!m@1372";
            var email = "pranc14@gmail.com";
            var roleName = "admin";

            var thisMethodName = nameof(SeedDatabaseWithAdminUserAsync);

            var adminUser = await _applicationUserManager.FindByNameAsync(name);
            if (adminUser != null)
            {
                _logger.LogInformation($"{thisMethodName}: adminUser already exists.");
                var role = _roleManager.GetRoleByUserGuid(adminUser.Id);
                if(_env.IsDevelopment() && role != null)
                    await _roleManager.AddOrUpdateRoleClaims(role.Id, GlobalEnum.DynamicRole, _mvcControllerDiscovery.GetAllAdminActionRoute());
                await _uow.SaveChangesAsync();
                return IdentityResult.Success;
            }

            //Create the `Admin` Role if it does not exist
            var adminRole = await _roleManager.FindByNameAsync(roleName);
            if (adminRole == null)
            {
                adminRole = new Role(roleName);
                adminRole.Id = new Guid();
                var adminRoleResult = await _roleManager.CreateAsync(adminRole);
                if (adminRoleResult == IdentityResult.Failed())
                {
                    _logger.LogError($"{thisMethodName}: adminRole CreateAsync failed. {adminRoleResult.DumpErrors()}");
                    return IdentityResult.Failed();
                }
                else
                {
                    await _roleManager.AddOrUpdateRoleClaims(adminRole.Id, GlobalEnum.DynamicRole, _mvcControllerDiscovery.GetAllAdminActionRoute());
                }
            }
            else
            {
                _logger.LogInformation($"{thisMethodName}: adminRole already exists.");
            }

            adminUser = new User
            {
                UserName = name,
                Email = email,
                EmailConfirmed = true,
                IsEmailPublic = true,
                LockoutEnabled = true
            };
            var adminUserResult = await _applicationUserManager.CreateAsync(adminUser, password);
            if (adminUserResult == IdentityResult.Failed())
            {
                _logger.LogError($"{thisMethodName}: adminUser CreateAsync failed. {adminUserResult.DumpErrors()}");
                return IdentityResult.Failed();
            }

            var setLockoutResult = await _applicationUserManager.SetLockoutEnabledAsync(adminUser, enabled: false);
            if (setLockoutResult == IdentityResult.Failed())
            {
                _logger.LogError($"{thisMethodName}: adminUser SetLockoutEnabledAsync failed. {setLockoutResult.DumpErrors()}");
                return IdentityResult.Failed();
            }

            var addToRoleResult = await _applicationUserManager.AddToRoleAsync(adminUser, adminRole.Name);
            if (addToRoleResult == IdentityResult.Failed())
            {
                _logger.LogError($"{thisMethodName}: adminUser AddToRoleAsync failed. {addToRoleResult.DumpErrors()}");
                return IdentityResult.Failed();
            }

            return IdentityResult.Success;
        }
    }
}