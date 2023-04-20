using Cargotruck.Server.Services.Interfaces;
using Cargotruck.Shared.Model.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace Cargotruck.Server.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class CargoesController : ControllerBase
    {
        private readonly ICargoService _cargoService;

        public CargoesController(ICargoService cargoService)
        {
            _cargoService = cargoService;
        }

        [HttpGet]
        public async Task<ActionResult<List<CargoDto>>> GetAsync(int page, int pageSize, string sortOrder,
            bool desc, string? searchString, string? filter, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate)
        {
            return Ok(await _cargoService.GetAsync(page, pageSize, sortOrder, desc, searchString, filter, dateFilterStartDate, dateFilterEndDate));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CargoDto>> GetByIdAsync(int id)
        {
            return Ok(await _cargoService.GetByIdAsync(id));
        }

        [HttpGet]
        public async Task<ActionResult<List<CargoDto>>> GetCargoesAsync()
        {
            return Ok(await _cargoService.GetCargoesAsync());
        }

        [HttpGet]
        public async Task<ActionResult<int[]>> GetChartDataAsync()
        {
            return Ok(await _cargoService.GetChartDataAsync());
        }

        [HttpGet]
        public async Task<ActionResult<int>> CountAsync(bool all)
        {
            return Ok(await _cargoService.CountAsync(all));
        }

        [HttpGet]
        public async Task<ActionResult<int>> PageCountAsync(string? searchString, string? filter, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate)
        {
            return Ok(await _cargoService.PageCountAsync(searchString, filter, dateFilterStartDate, dateFilterEndDate));
        }

        [HttpPost]
        public async Task PostAsync(CargoDto task)
        {
            await _cargoService.PostAsync(task);
        }

        [HttpPut]
        public async Task PutAsync(CargoDto task)
        {
            await _cargoService.PutAsync(task);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> DeleteAsync(int id)
        {
            return Ok(await _cargoService.DeleteAsync(id));
        }

        //closedXML needed !!!
        [HttpGet]
        public string ExportToExcel(CultureInfo lang, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate)
        {
            return _cargoService.ExportToExcel(lang, dateFilterStartDate, dateFilterEndDate);
        }

        //iTextSharp needed !!!
        [HttpGet]
        public async Task<string> ExportToPdfAsync(CultureInfo lang, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate)
        {
            return await _cargoService.ExportToPdfAsync(lang, dateFilterStartDate, dateFilterEndDate);
        }

        [HttpGet]
        public async Task<string> ExportToCSVAsync(CultureInfo lang, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate, bool isTextDocument)
        {
            return await _cargoService.ExportToCSVAsync(lang, dateFilterStartDate, dateFilterEndDate, isTextDocument);
        }


        [HttpPost]
        public async Task<ActionResult<string?>> ImportAsync([FromBody] string file, CultureInfo lang)
        {
            var result = await _cargoService.ImportAsync(file, lang);
            return (!string.IsNullOrEmpty(result) ? BadRequest(result) : Ok());
        }
    }
}