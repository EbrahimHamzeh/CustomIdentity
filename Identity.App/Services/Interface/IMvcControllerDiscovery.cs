
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Identity.App.ViewModel;

namespace Identity.App.Services.Interface
{
    public interface IMvcControllerDiscovery
    {
        IEnumerable<MvcControllerInfo> GetControllers();

        List<JsTreeNode> GetAdminActionInTree(string selectedIds = null);
    }
}
