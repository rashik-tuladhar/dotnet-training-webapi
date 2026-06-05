using Microsoft.AspNetCore.Mvc;

namespace DlmsWebApi.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
