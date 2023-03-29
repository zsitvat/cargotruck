using Cargotruck.Server.Data;
using Cargotruck.Server.Repositories.Interfaces;
using Cargotruck.Shared.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cargotruck.Server.Repositories
{
    public class PrivacyRepository : IPrivacyRepository
    {
        private readonly ApplicationDbContext _context;
        public PrivacyRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Privacies>> GetAsync(string lang)
        {
            return await _context.Privacies.Where(x => x.Lang == lang).ToListAsync();
        }

        public async Task<Privacies?> GetByIdAsync(int id)
        {
            return await _context.Privacies.FirstOrDefaultAsync(a => a.Id == id);
        }


        public async Task<int> CountAsync()
        {
            return await _context.Privacies.CountAsync();
        }

        public async Task PostAsync(Privacies data)
        {
            _context.Add(data);
            await _context.SaveChangesAsync();
        }

        public async Task PutAsync(Privacies data)
        {
            _context.Entry(data).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var data = _context.Privacies.FirstOrDefault(x => x.Id == id);

            if (data != null)
            {
                _context.Privacies.Remove(data);
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }
    }
}
