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

namespace Identity.App.Areas.Admin.Controllers
{
    //[Authorize]
    //[Authorize(Policy = GlobalEnum.DynamicRole)]
    [DisplayName("سطح دسترسی")]
    [Area(AreaConstants.AdminArea)]
    public class DynmicRoleController : Controller
    {
        private readonly IMvcControllerDiscovery _mvcControllerDiscovery;
        private readonly IApplicationRoleManager _roleManager;

        public DynmicRoleController(IMvcControllerDiscovery mvcControllerDiscovery, IApplicationRoleManager roleManager)
        {
            _mvcControllerDiscovery = mvcControllerDiscovery;
            _mvcControllerDiscovery.CheckArgumentIsNull(nameof(_mvcControllerDiscovery));

            _roleManager = roleManager;
            _roleManager.CheckArgumentIsNull(nameof(_roleManager));
        }

        [DisplayName("لیست")]
        public IActionResult Index()
        {
            return View();
        }

        [AjaxOnly]
        public async Task<IActionResult> Datalist()
        {
            return Json(await _roleManager.GetListAsync());
        }

        [DisplayName("افزودن")]
        public IActionResult Add()
        {
            return View(new DynmicRoleViewModel { JsonJSTree = JsonConvert.SerializeObject(_mvcControllerDiscovery.GetAdminActionInTree()) });
        }

        [ValidateAntiForgeryToken, HttpPost]
        public async Task<IActionResult> Add(DynmicRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(model.NodeSelected))
                {
                    var guid = Guid.NewGuid();
                    var role = new Role
                    {
                        Id = guid,
                        Name = model.Title,
                        Title = model.Title,
                        Enable = model.Enable,
                        Description = model.Description,
                        ActionArray = model.NodeSelected,
                    };

                    var result = await _roleManager.CreateAsync(role);

                    if (result.Succeeded)
                    {
                        await _roleManager.AddOrUpdateRoleClaims(guid, GlobalEnum.DynamicRole, model.NodeSelected);
                        return RedirectToAction(nameof(Index));
                    }
                    else
                        ModelState.AddErrorsFromResult(result);
                }
                else
                {
                    ModelState.AddModelError("JsonJSTree", "حداقل باید یک سطح دسترسی انتخاب کنید.");
                }
            }
            return View(model);
        }

        [DisplayName("ویرایش")]
        [Route("Admin/DynmicRole/Edit/{guid:guid:required}")]
        public async Task<IActionResult> Edit(Guid guid)
        {
            var role = await _roleManager.GetRoleByGuid(guid);
            role.JsonJSTree = JsonConvert.SerializeObject(_mvcControllerDiscovery.GetAdminActionInTree(role.NodeSelected));
            return View(role);
        }

        [ValidateAntiForgeryToken, HttpPost]
        [Route("Admin/DynmicRole/Edit/{guid:guid:required}")]
        public async Task<IActionResult> Edit(DynmicRoleViewModel model)
        {
            if (string.IsNullOrEmpty(model.NodeSelected))
                ModelState.AddModelError("JsonJSTree", "حداقل باید یک سطح دسترسی انتخاب کنید.");

            if (ModelState.IsValid)
            {
                var role = await _roleManager.FindByIdAsync(model.Guid.ToString());
                if (role == null)
                {
                    ModelState.AddModelError(string.Empty, "چنین سطح‌دسترسی در سیستم تعریف نشده است.");
                }
                else
                {
                    role.ActionArray = model.NodeSelected;
                    role.Name = role.Title = model.Title;
                    role.Description = model.Description;
                    role.Enable = model.Enable;
                    var result = await _roleManager.UpdateAsync(role);

                    if (result.Succeeded)
                    {
                        await _roleManager.AddOrUpdateRoleClaims(model.Guid, GlobalEnum.DynamicRole, model.NodeSelected);
                        return RedirectToAction(nameof(Index));
                    }
                    else
                        ModelState.AddErrorsFromResult(result);
                }
            }
            model.JsonJSTree = JsonConvert.SerializeObject(_mvcControllerDiscovery.GetAdminActionInTree(model.NodeSelected));
            return View(model);
        }
    }
}