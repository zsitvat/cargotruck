using Cargotruck.Data;
using Cargotruck.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

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
        public async Task<IActionResult> Get(int page, int pageSize, string sortOrder, bool desc)
        {
            var t = await _context.Tasks.ToListAsync();

            sortOrder = sortOrder == "Partner" ? ( desc ? "Partner_desc" : "Partner" ) : (sortOrder);
            sortOrder = sortOrder == "Description" ? (desc ? "Description_desc" : "Description") : (sortOrder);
            sortOrder = sortOrder == "Place_of_receipt" ? (desc ? "Place_of_receipt_desc" : "Place_of_receipt") : (sortOrder);
            sortOrder = sortOrder == "Time_of_receipt" ? (desc ? "Time_of_receipt_desc" : "Time_of_receipt") : (sortOrder);
            sortOrder = sortOrder == "Place_of_delivery" ? (desc ? "Place_of_delivery_desc" : "Place_of_delivery") : (sortOrder);
            sortOrder = sortOrder == "Time_of_delivery" ? (desc ? "Time_of_delivery_desc" : "Time_of_delivery") : (sortOrder);
            sortOrder = sortOrder == "Other_stops" ? (desc ? "Other_stops_desc" : "Other_stops") : (sortOrder);
            sortOrder = sortOrder == "Id_cargo" ? (desc ? "Id_cargo_desc" : "Id_cargo") : (sortOrder);
            sortOrder = sortOrder == "Storage_time" ? (desc ? "Storage_time_desc" : "Storage_time") : (sortOrder);
            sortOrder = sortOrder == "Completed" ? (desc ? "Completed_desc" : "Completed") : (sortOrder);
            sortOrder = sortOrder == "Completion_time" ? (desc ? "Completion_time_desc" : "Completion_time") : (sortOrder);
            sortOrder = sortOrder == "Time_of_delay" ? (desc ? "Time_of_delay_desc" : "Time_of_delay") : (sortOrder);
            sortOrder = sortOrder == "Payment" ? (desc ? "Payment_desc" : "Payment") : (sortOrder);
            sortOrder = sortOrder == "Final_Payment" ? (desc ? "Final_Payment_desc" : "Final_Payment") : (sortOrder);
            sortOrder = sortOrder == "Penalty" ? (desc ? "Penalty_desc" : "Penalty") : (sortOrder);
            sortOrder = sortOrder == "Date" || String.IsNullOrEmpty(sortOrder) ? (desc ? "Date_desc" : "") : (sortOrder);

            switch (sortOrder)
            {
                case "Partner_desc":
                    t = t.OrderByDescending(s => s.Partner).ToList();
                    break;
                case "Partner":
                    t = t.OrderBy(s => s.Partner).ToList();
                    break;
                case "Description_desc":
                    t = t.OrderByDescending(s => s.Description).ToList();
                    break;
                case "Description":
                    t = t.OrderBy(s => s.Description).ToList();
                    break;
                case "Place_of_receipt_desc":
                    t = t.OrderByDescending(s => s.Place_of_receipt).ToList();
                    break;
                case "Place_of_receipt":
                    t = t.OrderBy(s => s.Place_of_receipt).ToList();
                    break;
                case "Time_of_receipt_desc":
                    t = t.OrderByDescending(s => s.Time_of_receipt).ToList();
                    break;
                case "Time_of_receipt":
                    t = t.OrderBy(s => s.Time_of_receipt).ToList();
                    break;
                case "Place_of_delivery_desc":
                    t = t.OrderByDescending(s => s.Place_of_delivery).ToList();
                    break;
                case "Place_of_delivery":
                    t = t.OrderBy(s => s.Place_of_delivery).ToList();
                    break;
                case "Time_of_delivery_desc":
                    t = t.OrderByDescending(s => s.Time_of_delivery).ToList();
                    break;
                case "Time_of_delivery":
                    t = t.OrderBy(s => s.Time_of_delivery).ToList();
                    break;
                case "Other_stops_desc":
                    t = t.OrderByDescending(s => s.Other_stops).ToList();
                    break;
                case "Other_stops":
                    t = t.OrderBy(s => s.Other_stops).ToList();
                    break;
                case "Id_cargo_desc":
                    t = t.OrderByDescending(s => s.Id_cargo).ToList();
                    break;
                case "Id_cargo":
                    t = t.OrderBy(s => s.Id_cargo).ToList();
                    break;
                case "Storage_time_desc":
                    t = t.OrderByDescending(s => s.Storage_time).ToList();
                    break;
                case "Storage_time":
                    t = t.OrderBy(s => s.Storage_time).ToList();
                    break;
                case "Completed_desc":
                    t = t.OrderByDescending(s => s.Completed).ToList();
                    break;
                case "Completed":
                    t = t.OrderBy(s => s.Completed).ToList();
                    break;
                case "Completion_time_desc":
                    t = t.OrderByDescending(s => s.Completion_time).ToList();
                    break;
                case "Completion_time":
                    t = t.OrderBy(s => s.Completion_time).ToList();
                    break;
                case "Time_of_delay_desc":
                    t = t.OrderByDescending(s => s.Time_of_delay).ToList();
                    break;
                case "Time_of_delay":
                    t = t.OrderBy(s => s.Time_of_delay).ToList();
                    break;
                case "Payment_desc":
                    t = t.OrderByDescending(s => s.Payment).ToList();
                    break;
                case "Payment":
                    t = t.OrderBy(s => s.Payment).ToList();
                    break;
                case "Final_Payment_desc":
                    t = t.OrderByDescending(s => s.Final_Payment).ToList();
                    break;
                case "Final_Payment":
                    t = t.OrderBy(s => s.Final_Payment).ToList();
                    break;
                case "Penalty_desc":
                    t = t.OrderByDescending(s => s.Penalty).ToList();
                    break;
                case "Penalty":
                    t = t.OrderBy(s => s.Penalty).ToList();
                    break;
                case "Date_desc":
                    t = t.OrderByDescending(s => s.Date).ToList();
                    break;
                default:
                    t = t.OrderBy(s => s.Date).ToList();
                    break;
            }
            t = t.Skip((page - 1) * pageSize).Take(pageSize).ToList();
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