using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Identity.App.Areas;
using Identity.App.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Identity.App.Services
{
    public interface IMvcActionsDiscoveryService
    {
        List<MvcControllerInfo> MvcActions { get; }

        string GetAllAdminActionRoute();
    }

    public class MvcActionsDiscoveryService : IMvcActionsDiscoveryService
    {
        public MvcActionsDiscoveryService(IActionDescriptorCollectionProvider actionDescriptorCollectionProvider)
        {
            MvcActions = new List<MvcControllerInfo>();

            var items = actionDescriptorCollectionProvider
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
                MvcActions.Add(currentController);
            }
        }

        public List<MvcControllerInfo> MvcActions { get; }

        public string GetAllAdminActionRoute()
        {
            var allDetailRole = MvcActions;
            allDetailRole = allDetailRole?.Where(x => x.AreaName == AreaConstants.AdminArea).ToList();

            return string.Join(',', allDetailRole.Select(x=> string.Join(',', x.Actions.Select(y=> y.Id))));
        }
    }
}
