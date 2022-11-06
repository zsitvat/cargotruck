using Cargotruck.Data;
using Cargotruck.Server.Models;
using Cargotruck.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cargotruck.Server.Controllers
{
    [Route("api/[controller]/[action]")]
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
            var Claims = await  _context.UserClaims.ToDictionaryAsync(c => c.ClaimType + "/" + c.UserId, c => c.ClaimValue);
            return Ok(Claims);
        }

        public async Task<IActionResult> Roles()
        {
            var Roles = await _context.Roles.ToDictionaryAsync(r => r.Id, r => r.Name);
            var UsersRoles = await _context.UserRoles.ToDictionaryAsync(r => r.UserId, r => Roles[r.RoleId]);
            return Ok(UsersRoles);
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