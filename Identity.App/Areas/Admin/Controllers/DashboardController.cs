using System.ComponentModel;
using Microsoft.AspNetCore.Mvc;

namespace Identity.App.Areas.Admin.Controllers
{
    [DisplayName("داشبورد")]
     [Area(AreaConstants.AdminArea)]
    public class DashboardController : Controller
    {
        // [BreadCrumb(Title = "ایندکس", Order = 1)]
        [DisplayName("لیست")]
        public IActionResult Index() => View();
    }
}