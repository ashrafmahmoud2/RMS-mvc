using Microsoft.AspNetCore.Mvc;

namespace RMS.Web.Controllers;
public class DashboardController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
