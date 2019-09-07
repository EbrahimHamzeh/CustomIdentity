using System.Collections.Generic;

namespace Identity.App.ViewModel
{
    public class MvcControllerInfo
    {
        public string Id => $"{AreaName}:{Name}-Controller";

        public string Name { get; set; }

        public string DisplayName { get; set; }

        public string AreaName { get; set; }

        public IEnumerable<MvcActionInfo> Actions { get; set; }

    }
}
