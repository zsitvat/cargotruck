using Cargotruck.Server.Data;
using Cargotruck.Shared.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cargotruck.Server.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PrivacyController : Controller
    {
        private readonly ApplicationDbContext _context;
        public PrivacyController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync(string lang)
        {
            var data = await _context.Privacies.Where(x => x.Lang == lang).ToListAsync();
            return Ok(data);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var data = await _context.Privacies.FirstOrDefaultAsync(a => a.Id == id);
            return Ok(data);
        }

        [HttpGet]
        public async Task<IActionResult> CountAsync()
        {
            var t = await _context.Privacies.CountAsync();
            return Ok(t);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> PostAsync(Privacies data)
        {
            _context.Add(data);
            await _context.SaveChangesAsync();
            return Ok(data.Id);
        }

        [HttpPut]
        [Authorize]
        public async Task<IActionResult> PutAsync(Privacies data)
        {
            _context.Entry(data).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var data = new Privacies { Id = id };
            _context.Remove(data);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
