using Cargotruck.Server.Data;
using Cargotruck.Server.Models;
using Cargotruck.Server.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cargotruck.Server.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        private readonly ApplicationDbContext _context;
        public AdminRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<User>> GetAsync(int page, int pageSize, string? filter)
        {
            var u = await _context.Users.ToListAsync();

            if (filter != null && filter != "")
            {
                var Roles = await _context.Roles.ToDictionaryAsync(r => r.Id, r => r.Name);
                var UsersRoles = await _context.UserRoles.ToDictionaryAsync(r => r.UserId, r => Roles[r.RoleId]);

                u = u.Where(x => ((filter != null && filter != "") ? UsersRoles?[x.Id] == filter : true)).ToList();
            }

            return  u.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        }
        public async Task<User?> GetAsync(string id)
        {
            return await _context.Users.FirstOrDefaultAsync(a => a.Id == id);
        }
        public async Task<bool> DeleteAsync(string id)
        {
            var userForDelete = _context.Users.FirstOrDefault(x => x.Id == id);
            if (userForDelete != null)
            {
                _context.Users.Remove(userForDelete);
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }
        public async Task<int> PageCountAsync(string? filter)
        {
            var u = await _context.Users.ToListAsync();

            if (filter != null && filter != "")
            {
                var Roles = await _context.Roles.ToDictionaryAsync(r => r.Id, r => r.Name);
                var UsersRoles = await _context.UserRoles.ToDictionaryAsync(r => r.UserId, r => Roles[r.RoleId]);

                u = u.Where(x => ((filter != null && filter != "") ? UsersRoles?[x.Id] == filter : true)).ToList();
            }
            
            return  u.Count;
        }
        public async Task<int> CountAsync()
        {
            return await _context.Users.CountAsync();
        }
        public async Task<int> LoginsCountAsync()
        {
            return await _context.Logins.CountAsync();
        }
        public async Task<Dictionary<string, string>?> ClaimsAsync()
        {
            return await _context.UserClaims.ToDictionaryAsync(c => c.ClaimType + "/" + c.UserId, c => c.ClaimValue);
        }
        public async Task<Dictionary<string, string>?> RolesAsync()
        {
            var Roles = await _context.Roles.ToDictionaryAsync(r => r.Id, r => r.Name);
            return await _context.UserRoles.ToDictionaryAsync(r => r.UserId, r => Roles[r.RoleId]);
        }
    }
}
