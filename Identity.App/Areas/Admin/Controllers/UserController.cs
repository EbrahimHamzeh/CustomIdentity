
using Microsoft.AspNetCore.Mvc;
using Identity.App.Services.Interface;
using Identity.App.ViewModel;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Identity.App.Models;
using Identity.App.Extention;
using System;
using DNTCommon.Web.Core;
using Microsoft.AspNetCore.Authorization;
using Identity.App.Models.Context;
using Microsoft.EntityFrameworkCore;

namespace Identity.App.Areas.Admin.Controllers
{
    //[Authorize]
    [DisplayName("کاربران")]
    [Area(AreaConstants.AdminArea)]
    public class UserController : Controller
    {
        private readonly IApplicationRoleManager _roleManager;

        private readonly IApplicationUserManager _userManager;
        private readonly IUnitOfWork _uow;
        private readonly DbSet<UserRole> _userRoles;

        public UserController(IUnitOfWork uow, IApplicationUserManager userManager, IApplicationRoleManager roleManager)
        {
            _uow = uow;
            _uow.CheckArgumentIsNull(nameof(_uow));

            _userManager = userManager;
            _userManager.CheckArgumentIsNull(nameof(_userManager));

            _roleManager = roleManager;
            _roleManager.CheckArgumentIsNull(nameof(_roleManager));

            _userRoles = _uow.Set<UserRole>();
            _userRoles.CheckArgumentIsNull(nameof(_userRoles));
        }

        [DisplayName("لیست")]
        public IActionResult Index()
        {
            return View();
        }

        [AjaxOnly]
        public async Task<IActionResult> Datalist()
        {
            return Json(await _userManager.GetListAsync());
        }

        [DisplayName("افزودن")]
        public IActionResult Add()
        {
            return View(new UserViewModel { RolesSelectList = _roleManager.GetRolesSelectList() });
        }

        [ValidateAntiForgeryToken, HttpPost]
        public async Task<IActionResult> Add(UserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new User
                {
                    UserName = model.Username,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    EmailConfirmed = true,
                    IsActive = model.IsActive,
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    user = await _userManager.FindByNameAsync(model.Username);
                    var role = await _roleManager.FindByGuidAsync(model.RoleGuid);

                    await _userRoles.AddAsync(new UserRole { Role = role, User = user, RoleId = role.Id, UserId = user.Id });
                    await _uow.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
                else
                    ModelState.AddErrorsFromResult(result);
            }
            model.RolesSelectList = _roleManager.GetRolesSelectList();
            return View(model);
        }

        [DisplayName("ویرایش")]
        [Route("Admin/User/Edit/{guid:guid:required}")]
        public async Task<IActionResult> Edit(Guid guid)
        {
            var user = await _userManager.GetUserById(guid);
            user.RolesSelectList = _roleManager.GetRolesSelectList(user.RoleGuid);
            return View(user);
        }

        [ValidateAntiForgeryToken, HttpPost]
        [Route("Admin/User/Edit/{guid:guid:required}")]
        public async Task<IActionResult> Edit(UserViewModel model)
        {
            if (string.IsNullOrEmpty(model.Password) && string.IsNullOrEmpty(model.ConfirmPassword))
            {
                ModelState.Remove("Password");
                ModelState.Remove("ConfirmPassword");
            }
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.Guid.ToString());
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "چنین کاربری در سیستم تعریف نشده است.");
                }
                else
                {
                    user.UserName = model.Username;
                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    user.Email = model.Email;
                    user.IsActive = model.IsActive;
                    user.LockoutEnabled = model.LockoutEnabled;
                    user.EmailConfirmed = model.EmailConfirmed;
                    
                    if(!string.IsNullOrEmpty(model.Password) && !string.IsNullOrEmpty(model.Password))
                        user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, model.Password);

                    var result = await _userManager.UpdateAsync(user);

                    if (result.Succeeded)
                        return RedirectToAction(nameof(Index));
                    else
                        ModelState.AddErrorsFromResult(result);
                }
            }
            model.RolesSelectList = _roleManager.GetRolesSelectList(model.RoleGuid);
            return View(model);
        }
    }
}