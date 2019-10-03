
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using DNTCommon.Web.Core;
using Identity.App.Areas;
using Identity.App.Services.Interface;
using Identity.App.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Identity.App.Services
{
    public class MvcControllerDiscovery : IMvcControllerDiscovery
    {
        private List<MvcControllerInfo> _mvcControllers;
        private readonly IActionDescriptorCollectionProvider _actionDescriptorCollectionProvider;
        private readonly IMvcActionsDiscoveryService _mvcActionsDiscoveryService;

        public MvcControllerDiscovery(IActionDescriptorCollectionProvider actionDescriptorCollectionProvider, IMvcActionsDiscoveryService mvcActionsDiscoveryService)
        {
            _actionDescriptorCollectionProvider = actionDescriptorCollectionProvider;
            _mvcActionsDiscoveryService = mvcActionsDiscoveryService;
        }

        public IEnumerable<MvcControllerInfo> GetControllers()
        {
            if (_mvcControllers != null)
                return _mvcControllers;

            _mvcControllers = new List<MvcControllerInfo>();

            var items = _actionDescriptorCollectionProvider
                .ActionDescriptors.Items
                .Where(descriptor => descriptor.GetType() == typeof(ControllerActionDescriptor))
                .Select(descriptor => (ControllerActionDescriptor)descriptor)
                .GroupBy(descriptor => descriptor.ControllerTypeInfo.FullName)
                .ToList();

            foreach (var actionDescriptors in items)
            {
                if (!actionDescriptors.Any())
                    continue;

                var actionDescriptor = actionDescriptors.First();
                var controllerTypeInfo = actionDescriptor.ControllerTypeInfo;
                var currentController = new MvcControllerInfo
                {
                    AreaName = controllerTypeInfo.GetCustomAttribute<AreaAttribute>()?.RouteValue,
                    DisplayName = controllerTypeInfo.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName,
                    Name = actionDescriptor.ControllerName,
                };

                var actions = new List<MvcActionInfo>();
                foreach (var descriptor in actionDescriptors.GroupBy
                                            (a => a.ActionName).Select(g => g.First()))
                {
                    var methodInfo = descriptor.MethodInfo;
                    actions.Add(new MvcActionInfo
                    {
                        ControllerId = currentController.Id,
                        Name = descriptor.ActionName,
                        DisplayName =
                             methodInfo.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName,
                    });
                }

                currentController.Actions = actions;
                _mvcControllers.Add(currentController);
            }

            return _mvcControllers;
        }

        public List<JsTreeNode> GetAdminActionInTree(string selectedIds){
            var allDetailRole = this.GetControllers();
            allDetailRole = allDetailRole?.Where(x => x.AreaName == AreaConstants.AdminArea).ToList();

            string[] selectedActionsId = null;
            if (!string.IsNullOrEmpty(selectedIds))
                selectedActionsId = selectedIds.Split(',');

            var nodesList = new List<JsTreeNode>();
            foreach (var parent in allDetailRole)
            {
                var actionNodesList = new List<JsTreeNode>();
                foreach (var item in parent.Actions)
                {
                    var actionNode = new JsTreeNode
                    {
                        id = item.Id,
                        text = item.DisplayName,
                        state = new JsTreeNodeState { selected = selectedActionsId == null ? false : selectedActionsId.Contains(item.Id) }
                    };
                    actionNodesList.Add(actionNode);
                }

                var rootNode = new JsTreeNode
                {
                    id = parent.Id,
                    text = parent.DisplayName,
                    children = actionNodesList,
                    state = new JsTreeNodeState { selected = selectedActionsId == null ? false : selectedActionsId.Contains(parent.Id) }
                };
                nodesList.Add(rootNode);
            }

            return nodesList;
        } 

        public string GetAllAdminActionRoute(){
            var allDetailRole = this.GetControllers();
            allDetailRole = allDetailRole?.Where(x => x.AreaName == AreaConstants.AdminArea).ToList();

            var controllerActions = _mvcActionsDiscoveryService.MvcControllers;
            controllerActions = controllerActions?.Where(x => x.AreaName == AreaConstants.AdminArea).ToList();

            return string.Join(',', controllerActions.Select(x=> string.Join(',', x.MvcActions.Select(y=> y.ActionId))));
        }
    }
}
