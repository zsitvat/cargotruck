using Cargotruck.Data;
using Cargotruck.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Cargotruck.Server.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public TasksController(ApplicationDbContext context)
        {
            this._context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get(int page, int pageSize)
        {
            var t = await _context.Tasks.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            return Ok(t);
        }

        [HttpGet]
        public async Task<IActionResult> PageCount()
        {
            var t = await _context.Tasks.ToListAsync();
            var PageCount = t.Count();
            return Ok(PageCount);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var t = await _context.Tasks.FirstOrDefaultAsync(a => a.Id == id);
            return Ok(t);
        }

        [HttpPost]
        public async Task<IActionResult> Post(Tasks t)
        {
            _context.Add(t);
            await _context.SaveChangesAsync();
            return Ok(t.Id);
        }

        [HttpPut]
        public async Task<IActionResult> Put(Tasks t)
        {
            _context.Entry(t).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var t = new Tasks { Id = id };
            _context.Remove(t);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}