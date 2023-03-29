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
    public class RoadsController : ControllerBase
    {
        private readonly IRoadService _roadService;

        public RoadsController(IRoadService roadService)
        {
            _roadService = roadService;
        }

        [HttpGet]
        public async Task<ActionResult<List<RoadsDto>>> GetAsync(int page, int pageSize, string sortOrder,
            bool desc, string? searchString, string? filter, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate)
        {
            return Ok(await _roadService.GetAsync(page, pageSize, sortOrder, desc, searchString, filter, dateFilterStartDate, dateFilterEndDate));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RoadsDto>> GetByIdAsync(int id)
        {
            return Ok(await _roadService.GetByIdAsync(id));
        }

        [HttpGet]
        public async Task<ActionResult<List<RoadsDto>>> GetRoadsAsync()
        {
            return Ok(await _roadService.GetRoadsAsync());
        }

        [HttpGet]
        public async Task<ActionResult<int[]>> GetChartDataAsync()
        {
            return Ok(await _roadService.GetChartDataAsync());
        }

        [HttpGet]
        public async Task<ActionResult<int>> CountAsync()
        {
            return Ok(await _roadService.CountAsync());
        }

        [HttpGet]
        public async Task<ActionResult<int>> PageCountAsync(string? searchString, string? filter, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate)
        {
            return Ok(await _roadService.PageCountAsync(searchString, filter, dateFilterStartDate, dateFilterEndDate));
        }

        [HttpPost]
        public async Task PostAsync(RoadsDto road)
        {
            await _roadService.PostAsync(road);
        }

        [HttpPut]
        public async Task PutAsync(RoadsDto road)
        {
            await _roadService.PutAsync(road);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> DeleteAsync(int id)
        {
            return Ok(await _roadService.DeleteAsync(id));
        }

        //closedXML needed !!!
        [HttpGet]
        public string ExportToExcel(CultureInfo lang, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate)
        {
            return _roadService.ExportToExcel(lang, dateFilterStartDate, dateFilterEndDate);
        }

        //iTextSharp needed !!!
        [HttpGet]
        public async Task<string> ExportToPdfAsync(CultureInfo lang, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate)
        {
            return await _roadService.ExportToPdfAsync(lang, dateFilterStartDate, dateFilterEndDate);
        }

        [HttpGet]
        public async Task<string> ExportToCSVAsync(CultureInfo lang, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate, bool isTextDocument)
        {
            return await _roadService.ExportToCSVAsync(lang, dateFilterStartDate, dateFilterEndDate, isTextDocument);
        }


        [HttpPost]
        public async Task<ActionResult<string?>> ImportAsync([FromBody] string file, CultureInfo lang)
        {
            var result = await _roadService.ImportAsync(file, lang);
            return (result != null ? BadRequest(result) : Ok());
        }
    }
}