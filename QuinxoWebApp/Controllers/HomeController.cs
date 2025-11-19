using Microsoft.AspNetCore.Mvc;

namespace QuinxoWebApp.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
