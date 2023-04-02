 using Cargotruck.Server.Data;
using Cargotruck.Shared.Model.Dto;
using Microsoft.AspNetCore.Authorization;
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
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<UserDto>>> GetAsync(int page, int pageSize, string? filter)
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

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserDto?>> GetAsync(string id)
        {
            var u = await _context.Users.FirstOrDefaultAsync(a => a.Id == id);

            return Ok(u);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<bool>> DeleteAsync(string id)
        {
            var userForDelete = _context.Users.FirstOrDefault(a => a.Id == id);

            _context?.RemoveRange(userForDelete!);
            await _context?.SaveChangesAsync()!;

            return NoContent();
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<int>> PageCountAsync(string? filter)
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

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<int>> CountAsync()
        {
            var u = await _context.Users.CountAsync();

            return Ok(u);
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<int>> LoginsCountAsync()
        {
            var u = await _context.Logins.CountAsync();

            return Ok(u);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Dictionary<string, string>?>> ClaimsAsync()
        {
            var Claims = await _context.UserClaims.ToDictionaryAsync(c => c.ClaimType + "/" + c.UserId, c => c.ClaimValue);
            
            return Ok(Claims);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Dictionary<string, string>?>> RolesAsync()
{
            var Roles = await _context.Roles.ToDictionaryAsync(r => r.Id, r => r.Name);
            var UsersRoles = await _context.UserRoles.ToDictionaryAsync(r => r.UserId, r => Roles[r.RoleId]);
            
            return Ok(UsersRoles);
        }

    }
}