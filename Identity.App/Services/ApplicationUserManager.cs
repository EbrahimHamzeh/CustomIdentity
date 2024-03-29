using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Identity.App.Extention;
using Identity.App.Models;
using Identity.App.Models.Context;
using Identity.App.Services.Interface;
using Identity.App.ViewModel;
using Identity.App.ViewModel.Paged;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Identity.App.Services
{
    public class ApplicationUserManager : UserManager<User>, IApplicationUserManager
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IUnitOfWork _uow;

        private readonly DbSet<User> _users;

        private User _currentUserInScope;
        private List<User> _currentUserRolesInScope;

        public ApplicationUserManager(IApplicationUserStore store,
            IOptions<IdentityOptions> optionsAccessor,
            IPasswordHasher<User> passwordHasher,
            IEnumerable<IUserValidator<User>> userValidators,
            IEnumerable<IPasswordValidator<User>> passwordValidators,
            ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors,
            IServiceProvider services,
            ILogger<UserManager<User>> logger,
            IUnitOfWork uow,
            IHttpContextAccessor contextAccessor)
                : base((UserStore<User, Role, AppDbContext, Guid, UserClaim, UserRole, UserLogin, UserToken, RoleClaim>)store,
                optionsAccessor,
                passwordHasher,
                userValidators,
                passwordValidators,
                keyNormalizer,
                errors,
                services,
                logger)
        {
            _uow = uow;
            _uow.CheckArgumentIsNull(nameof(_uow));

            _contextAccessor = contextAccessor;
            _contextAccessor.CheckArgumentIsNull(nameof(_contextAccessor));

            _users = uow.Set<User>();
        }

        string IApplicationUserManager.CreateTwoFactorRecoveryCode()
        {
            return base.CreateTwoFactorRecoveryCode();
        }

        Task<PasswordVerificationResult> IApplicationUserManager.VerifyPasswordAsync(IUserPasswordStore<User> store, User user, string password)
        {
            return base.VerifyPasswordAsync(store, user, password);
        }

        public User FindById(Guid userId)
        {
            return _users.Find(userId);
        }

        public User GetCurrentUser()
        {
            if (_currentUserInScope != null)
            {
                return _currentUserInScope;
            }

            var currentUserId = GetCurrentUserId();
            if (string.IsNullOrWhiteSpace(currentUserId))
            {
                return null;
            }

            var userId = Guid.Parse(currentUserId);
            return _currentUserInScope = FindById(userId);
        }

        public async Task<User> GetCurrentUserAsync()
        {
            return _currentUserInScope ??
                (_currentUserInScope = await GetUserAsync(_contextAccessor.HttpContext.User));
        }

        public string GetCurrentUserId()
        {
            return _contextAccessor.HttpContext.User.Identity.GetUserId();
        }

        public List<User> GetCurrentUserRoles()
        {
            if (_currentUserRolesInScope != null)
                return _currentUserRolesInScope;

            var currentUserId = GetCurrentUserId();
            if (string.IsNullOrWhiteSpace(currentUserId))
            {
                return null;
            }

            var userId = Guid.Parse(currentUserId);

            return _currentUserRolesInScope = _users.Where(x => x.Id == userId).Include(x => x.Roles).ThenInclude(y => y.Role).ToList();
        }

        public string GetCurrentUserAccessInAction()
        {
            var roles = GetCurrentUserRoles();
            if (roles != null)
            {
                return string.Join(',', roles.FirstOrDefault().Roles.Select(x => x.Role.ActionArray).ToList());
            }

            return null;
        }

        public async Task<PagedQueryResult<UserListViewModel>> GetListAsync()
        {
            var query = Users.AsNoTracking().AsQueryable();

            var total = await query.CountAsync();

            var data = (await query.ToListAsync()).Select(x =>
                new UserListViewModel
                {
                    Guid = x.Id,
                    Username = x.UserName,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Email = x.Email,
                    IsActive = x.IsActive,
                    LockoutEnd = x.LockoutEnd,
                    EmailConfirmed = x.EmailConfirmed,
                    LockoutEnabled = x.LockoutEnabled,
                    TwoFactorEnabled = x.TwoFactorEnabled
                }).ToList();

            return new PagedQueryResult<UserListViewModel> { Total = total, Rows = data };
        }

        public async Task<UserViewModel> GetUserById(Guid guid = new Guid())
        {
            if (guid == Guid.Empty) return null;

            var user = await _users.Where(x => x.Id == guid).Include(x => x.Roles).FirstOrDefaultAsync();
            if (user != null)
            {
                return new UserViewModel
                {
                    Guid = user.Id,
                    Username = user.UserName,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    IsActive = user.IsActive,
                    EmailConfirmed = user.EmailConfirmed,
                    LockoutEnabled = user.LockoutEnabled,
                    TwoFactorEnabled = user.TwoFactorEnabled,
                    RoleGuid = user.Roles.FirstOrDefault() != null ? user.Roles.FirstOrDefault().RoleId : Guid.Empty
                };
            }
            return null;
        }

        public async Task<IdentityResult> UpdateUserAndSecurityStampAsync(Guid id, Action<User> action)
        {
            var user = await _users.FindAsync(id);
            if (user != null)
            {
                action(user);

                var result = await UpdateAsync(user);
                if (!result.Succeeded)
                {
                    return result;
                }
                return await UpdateSecurityStampAsync(user);
            }

            return IdentityResult.Failed(new IdentityError
            {
                Code = "UserNotFound",
                Description = "کاربر مورد نظر یافت نشد."
            });
        }
    }
}
