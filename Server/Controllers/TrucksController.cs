using Cargotruck.Server.Data;
using Cargotruck.Server.Services;
using Cargotruck.Shared.Model;
using Cargotruck.Shared.Resources;
using ClosedXML.Excel;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.Data;
using System.Globalization;
using System.Text;
using Document = iTextSharp.text.Document;
using Font = iTextSharp.text.Font;

namespace Cargotruck.Server.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class TrucksController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IStringLocalizer<Resource> _localizer;
        private readonly IColumnNamesService _columnNameLists;

        public TrucksController(ApplicationDbContext context, IStringLocalizer<Resource> localizer, IColumnNamesService columnNameLists)
        {
            _context = context;
            _localizer = localizer;
            _columnNameLists = columnNameLists;
        }

        private async Task<List<Trucks>> GetDataAsync(string? searchString, Status? filter, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate)
        {
           
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync(int page, int pageSize, string sortOrder, bool desc, string? searchString, Status? filter, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate)
        {
           
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var data = 
            return Ok(data);
        }

        [HttpGet]
        public async Task<IActionResult> GetTrucksAsync()
        {
            var data = 
            return Ok(data);
        }

        [HttpGet("{vehicle_registration_number}")]
        public async Task<IActionResult> GetByVRNAsync(string vehicle_registration_number)
        {
            var data = await _context.Trucks.FirstOrDefaultAsync(a => a.Vehicle_registration_number == vehicle_registration_number);
            return Ok(data);
        }

        [HttpGet]
        public async Task<IActionResult> CountAsync(bool all)
        {
            
        }

        [HttpGet]
        public async Task<IActionResult> PageCountAsync(string? searchString, Status? filter, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate)
        {
           
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync(Trucks data)
        {
            data.User_id = _context.Users.FirstOrDefault(a => a.UserName == User.Identity!.Name)?.Id;
            _context.Add(data);
            await _context.SaveChangesAsync();
            return Ok(data.Id);
        }

        [HttpPut]
        public async Task<IActionResult> PutAsync(Trucks data)
        {
            _context.Entry(data).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var data = new Trucks { Id = id };
            _context.Remove(data);
            await _context.SaveChangesAsync();
            return NoContent();
        }


        //closedXML needed !!!
        [HttpGet]
        public string ExportToExcel(CultureInfo lang, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate)
        {
            
        }

        //iTextSharp needed !!!
        [HttpGet]
        public async Task<string> ExportToPdfAsync(CultureInfo lang, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate)
        {
            
        }

        //iTextSharp needed !!!
        [HttpGet]
        public async Task<string> ExportToCSVAsync(CultureInfo lang, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate, bool isTextDocument)
        {
            
        }

        [HttpPost]
        public async Task<IActionResult> ImportAsync([FromBody] string file, CultureInfo lang)
        {
            
    }
}