using Cargotruck.Shared;
using Microsoft.AspNetCore.Mvc;
using Cargotruck.Data;

namespace Cargotruck.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MainController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public MainController(ApplicationDbContext context)
        {
            _context = context;
        }
    }

}