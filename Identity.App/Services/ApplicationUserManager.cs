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
                : base((UserStore<User, Role, AppDbContext, int, UserClaim, UserRole, UserLogin, UserToken, RoleClaim>)store,
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

        public User FindById(int userId)
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

            var userId = int.Parse(currentUserId);
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

            var userId = int.Parse(currentUserId);

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
                    Guid = x.Guid,
                    Username = x.UserName,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Email = x.Email,
                    IsActive = x.IsActive,
                }).ToList();

            return new PagedQueryResult<UserListViewModel> { Total = total, Rows = data };
        }

    }
}
