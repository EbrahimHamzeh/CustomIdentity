namespace Identity.App.ViewModel
{
    public class MvcActionInfo
    {
        public string Id => $"{ControllerId}:{Name}-Action";

        public string Name { get; set; }

        public string DisplayName { get; set; }

        public string ControllerId { get; set; }

    }
}
