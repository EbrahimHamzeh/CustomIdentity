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
        public ApplicationRoleManager(
            IApplicationRoleStore store,
            IEnumerable<IRoleValidator<Role>> roleValidators,
            ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors,
            ILogger<ApplicationRoleManager> logger,
            IHttpContextAccessor contextAccessor) :
            base((RoleStore<Role, AppDbContext, int, UserRole, RoleClaim>)store, roleValidators, keyNormalizer, errors, logger)
        {

            _contextAccessor = contextAccessor;
            _contextAccessor.CheckArgumentIsNull(nameof(_contextAccessor));
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
                    Guid = x.Guid,
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
                Guid = rol.Guid,
                Title = rol.Title,
                NodeSelected = rol.ActionArray,
                Description = rol.Description,
                Enable = rol.Enable
            };
        }

        public async Task<Role> FindByGuidAsync(Guid guid){
            if (guid == Guid.Empty)
                return null;
            return await Roles.FirstOrDefaultAsync(x=> x.Guid == guid);
        }

        private int getCurrentUserId() => _contextAccessor.HttpContext.User.Identity.GetUserId<int>();

        public List<SelectListItem> GetRolesSelectList(){
            return Roles.Select(x=> new SelectListItem {
                Value = x.Guid.ToString(),
                Text = x.Name
            }).ToList();
        }

    }
}
