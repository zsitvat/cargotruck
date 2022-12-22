using Microsoft.AspNetCore.Mvc;

namespace Cargotruck.Server.Controllers
{
    public class TrucksController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
