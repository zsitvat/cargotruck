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
        public async Task<IActionResult> Get(int page, int pageSize, string? filter)
        {
            var u = await _context.Users.ToListAsync();

            if (filter != null && filter != "")
            {
                var Roles = await _context.Roles.ToDictionaryAsync(r => r.Id, r => r.Name);
                var UsersRoles = await _context.UserRoles.ToDictionaryAsync(r => r.UserId, r => Roles[r.RoleId]);
                u = u.Where(x => ((filter != null && filter != "") ? UsersRoles?[x.Id] == filter : true)).ToList();
            }

            u = u.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            return Ok(u);
        }

        [HttpGet]
        public async Task<IActionResult> Count()
        {
            var u = await _context.Users.CountAsync();
            return Ok(u);
        }

        [HttpGet]
        public async Task<IActionResult> LoginsCount()
        {
            var u = await _context.Logins.CountAsync();
            return Ok(u);
        }

        [HttpGet]
        public async Task<IActionResult> PageCount(string? filter)
        {
            var u = await _context.Users.ToListAsync();
            if (filter != null && filter != "")
            {
                var Roles = await _context.Roles.ToDictionaryAsync(r => r.Id, r => r.Name);
                var UsersRoles = await _context.UserRoles.ToDictionaryAsync(r => r.UserId, r => Roles[r.RoleId]);
                u = u.Where(x => ((filter != null && filter != "") ? UsersRoles?[x.Id] == filter : true)).ToList();
            }
            int PageCount = u.Count;
            return Ok(PageCount);
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
            var userForDelete = _context.Users.FirstOrDefault(a => a.Id == id);
            _context.Remove(userForDelete);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}