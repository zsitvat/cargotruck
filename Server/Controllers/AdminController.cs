using Cargotruck.Data;
using Cargotruck.Server.Models;
using Cargotruck.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cargotruck.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        public AdminController(ApplicationDbContext context)
        {
            this._context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var u = await _context.Users.ToListAsync();
            return Ok(u);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var u = await _context.Users.FirstOrDefaultAsync(a => a.Id == id);
            return Ok(u);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var u = new Users { Id = id };
            _context.Remove(u);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
