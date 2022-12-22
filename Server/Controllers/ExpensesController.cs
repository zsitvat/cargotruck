using Microsoft.AspNetCore.Mvc;

namespace Cargotruck.Server.Controllers
{
    public class ExpensesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
