using Cargotruck.Data;
using Cargotruck.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cargotruck.Server.Controllers
{
    [Route("api/Tasks")]
    [ApiController]
    public class TasksController : Controller
    {
        private readonly ApplicationDbContext _context;
        public TasksController(ApplicationDbContext context)
        {
            this._context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var t = await _context.Tasks.ToListAsync();
            return Ok(t);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var t = await _context.Tasks.FirstOrDefaultAsync(a => a.Id == id);
            return Ok(t);
        }

        [HttpPost]
        public async Task<IActionResult> Post(Tasks Task)
        {
            _context.Add(Task);
            await _context.SaveChangesAsync();
            return Ok(Task.Id);
        }

        [HttpPut]
        public async Task<IActionResult> Put(Tasks Task)
        {
            _context.Entry(Task).State = EntityState.Modified;
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
