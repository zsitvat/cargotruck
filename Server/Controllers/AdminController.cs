using Cargotruck.Data;
using Cargotruck.Server.Models;
using Cargotruck.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cargotruck.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
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
        public async Task<IActionResult> Claims()
        {
            var Claims = User.Claims.ToDictionary(c => c.Type, c => c.Value);
             return Ok(Claims);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            //var u = new Users { Id = id };
            var u = _context.Users.FirstOrDefault(a => a.Id == id);
            _context.Remove(u);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
