using Cargotruck.Server.Services;
using Cargotruck.Shared.Model;
using Cargotruck.Shared.Model.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace Cargotruck.Server.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class TrucksController : ControllerBase
    {
        private readonly ITruckService _truckService;

        public TrucksController(ITruckService truckService)
        {
            _truckService = truckService;
        }

        [HttpGet]
        public async Task<ActionResult<List<TrucksDto>>> GetAsync(int page, int pageSize, string sortOrder,
            bool desc, string? searchString, Status? filter, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate)
        {
            return Ok(await _truckService.GetAsync(page, pageSize, sortOrder, desc, searchString, filter, dateFilterStartDate, dateFilterEndDate));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TrucksDto>> GetByIdAsync(int id)
        {
            return Ok(await _truckService.GetByIdAsync(id));
        }

        [HttpGet("{vehicle_registration_number}")]
        public async Task<ActionResult<TrucksDto>> GetByVRNAsync(string vehicle_registration_number)
        {
            return Ok(await _truckService.GetByVRNAsync(vehicle_registration_number));
        }

        [HttpGet]
        public async Task<ActionResult<List<TrucksDto>>> GetTrucksAsync()
        {
            return Ok(await _truckService.GetTrucksAsync());
        }

        [HttpGet]
        public async Task<ActionResult<int>> CountAsync(bool all)
        {
            return Ok(await _truckService.CountAsync(all));
        }

        [HttpGet]
        public async Task<ActionResult<int>> PageCountAsync(string? searchString, Status? filter, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate)
        {
            return Ok(await _truckService.PageCountAsync(searchString,filter, dateFilterStartDate, dateFilterEndDate));
        }

        [HttpPost]
        public async Task PostAsync(TrucksDto task)
        {
            await _truckService.PostAsync(task);
        }

        [HttpPut]
        public async Task PutAsync(TrucksDto task)
        {
            await _truckService.PutAsync(task);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            return Ok(await _truckService.DeleteAsync(id));
        }

        //closedXML needed !!!
        [HttpGet]
        public string ExportToExcel(CultureInfo lang, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate)
        {
            return _truckService.ExportToExcel(lang, dateFilterStartDate, dateFilterEndDate);
        }

        //iTextSharp needed !!!
        [HttpGet]
        public async Task<string> ExportToPdfAsync(CultureInfo lang, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate)
        {
            return await _truckService.ExportToPdfAsync(lang, dateFilterStartDate, dateFilterEndDate);
        }

        [HttpGet]
        public async Task<string> ExportToCSVAsync(CultureInfo lang, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate, bool isTextDocument)
        {
            return await _truckService.ExportToCSVAsync(lang, dateFilterStartDate, dateFilterEndDate, isTextDocument);
        }


        [HttpPost]
        public async Task<ActionResult<string?>> ImportAsync([FromBody] string file, CultureInfo lang)
        {
            var result = await _truckService.ImportAsync(file, lang);
            return (result != null ? result : Ok());
        }
    }
}