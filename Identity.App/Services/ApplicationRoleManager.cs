using System.Collections.Generic;
using Identity.App.Extention;
using Identity.App.Models;
using Identity.App.Models.Context;
using Identity.App.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Identity.App.ViewModel;
using System.Threading.Tasks;
using System.Linq;
using Identity.App.ViewModel.Paged;
using Microsoft.EntityFrameworkCore;
using System;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Identity.App.Services
{
    public class ApplicationRoleManager : RoleManager<Role>, IApplicationRoleManager
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly DbSet<RoleClaim> _roleClaims;
        private readonly IUnitOfWork _uow;

        public ApplicationRoleManager(
            IApplicationRoleStore store,
            IEnumerable<IRoleValidator<Role>> roleValidators,
            ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors,
            ILogger<ApplicationRoleManager> logger,
            IHttpContextAccessor contextAccessor,
            IUnitOfWork uow) :
            base((RoleStore<Role, AppDbContext, Guid, UserRole, RoleClaim>)store, roleValidators, keyNormalizer, errors, logger)
        {
            _contextAccessor = contextAccessor;
            _contextAccessor.CheckArgumentIsNull(nameof(_contextAccessor));

            _uow = uow;
            _uow.CheckArgumentIsNull(nameof(_uow));

            _roleClaims = _uow.Set<RoleClaim>();
        }

        public async Task<PagedQueryResult<DynmicRoleListViewModel>> GetListAsync()
        {
            var query = Roles.AsNoTracking().AsQueryable();

            // TODO: Apply Filtering ... .where(p => p....) ...

            // var columnsMap = new Dictionary<string, Expression<Func<Tiket, object>>> () {
            //         ["title"] = p => p.Title, ["date"] = p => p.Date, ["status"] = p => p.Status
            //     };

            // query = query.ApplyOrdering (queryModel, columnsMap);
            var total = await query.CountAsync();
            // query = query.ApplyPaging (queryModel);
            var data = (await query.ToListAsync()).Select(x =>
                new DynmicRoleListViewModel
                {
                    Guid = x.Id,
                    Title = x.Name,
                    Description = x.Description,
                    Enable = x.Enable
                }).ToList();

            return new PagedQueryResult<DynmicRoleListViewModel> { Total = total, Rows = data };
        }

        public async Task<DynmicRoleViewModel> GetRoleByGuid(Guid guid)
        {
            if (guid == Guid.Empty)
                return null;
            
            var rol = await this.FindByGuidAsync(guid);

            return new DynmicRoleViewModel {
                Guid = rol.Id,
                Title = rol.Title,
                NodeSelected = rol.ActionArray,
                Description = rol.Description,
                Enable = rol.Enable
            };
        }

        public async Task<Role> FindByGuidAsync(Guid guid){
            if (guid == Guid.Empty)
                return null;
            return await Roles.FirstOrDefaultAsync(x=> x.Id == guid);
        }

        private int getCurrentUserId() => _contextAccessor.HttpContext.User.Identity.GetUserId<int>();

        public List<SelectListItem> GetRolesSelectList(Guid id = new Guid()){
            return Roles.Select(x=> new SelectListItem {
                Value = x.Id.ToString(),
                Text = x.Name,
                Selected = x.Id == id
            }).ToList();
        }

        public async Task AddOrUpdateRoleClaims(Guid roleId, string roleClaimType, string selectedRoleClaimValues)
        {
            var roleClaim = _roleClaims.Where(x => x.ClaimType == roleClaimType && x.RoleId == roleId).FirstOrDefault();

            if(roleClaim != null){
                roleClaim.RoleId = roleId;
                roleClaim.ClaimType = roleClaimType;
                roleClaim.ClaimValue = selectedRoleClaimValues;

                _roleClaims.Update(roleClaim);
            } else {
                roleClaim = new RoleClaim {
                    RoleId = roleId,
                    ClaimType = roleClaimType,
                    ClaimValue = selectedRoleClaimValues
                };
                await _roleClaims.AddAsync(roleClaim);

            }

            await _uow.SaveChangesAsync();
        }
    }
}
