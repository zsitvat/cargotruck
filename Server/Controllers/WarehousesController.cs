using Cargotruck.Server.Data;
using Cargotruck.Server.Services;
using Cargotruck.Shared.Model;
using Cargotruck.Shared.Model.Dto;
using Cargotruck.Shared.Resources;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.Data;
using System.Globalization;
using System.Linq.Dynamic.Core;
using System.Text;
using Document = iTextSharp.text.Document;
using Font = iTextSharp.text.Font;

namespace Cargotruck.Server.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class WarehousesController : ControllerBase
    {
        private readonly IWarehouseService _warehouseService;

        public WarehousesController(IWarehouseService warehouseService)
        {
            _warehouseService = warehouseService;
        }

        [HttpGet]
        public async Task<ActionResult<List<WarehousesDto>>> GetAsync(int page, int pageSize, string sortOrder,
            bool desc, string? searchString, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate)
        {
            return Ok(await _warehouseService.GetAsync(page, pageSize, sortOrder, desc, searchString, dateFilterStartDate, dateFilterEndDate));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<WarehousesDto>> GetByIdAsync(int id)
        {
            return Ok(await _warehouseService.GetByIdAsync(id));
        }

        [HttpGet]
        public async Task<ActionResult<List<WarehousesDto>>> GetWarehousesAsync()
        {
            return Ok(await _warehouseService.GetWarehousesAsync());
        }

        [HttpGet]
        public async Task<ActionResult<int>> CountAsync(bool all)
        {
            return Ok(await _warehouseService.CountAsync(all));
        }

        [HttpGet]
        public async Task<ActionResult<int>> PageCountAsync(string? searchString, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate)
        {
            return Ok(await _warehouseService.PageCountAsync(searchString, dateFilterStartDate, dateFilterEndDate));
        }

        [HttpPost]
        public async Task PostAsync(WarehousesDto task)
        {
            await _warehouseService.PostAsync(task);
        }

        [HttpPut]
        public async Task PutAsync(WarehousesDto task)
        {
            await _warehouseService.PutAsync(task);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            return Ok(await _warehouseService.DeleteAsync(id));
        }

        //closedXML needed !!!
        [HttpGet]
        public string ExportToExcel(CultureInfo lang, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate)
        {
            return _warehouseService.ExportToExcel(lang, dateFilterStartDate, dateFilterEndDate);
        }

        //iTextSharp needed !!!
        [HttpGet]
        public async Task<string> ExportToPdfAsync(CultureInfo lang, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate)
        {
            return await _warehouseService.ExportToPdfAsync(lang, dateFilterStartDate, dateFilterEndDate);
        }

        [HttpGet]
        public async Task<string> ExportToCSVAsync(CultureInfo lang, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate, bool isTextDocument)
        {
            return await _warehouseService.ExportToCSVAsync(lang, dateFilterStartDate, dateFilterEndDate, isTextDocument);
        }


        [HttpPost]
        public async Task<ActionResult<string?>> ImportAsync([FromBody] string file, CultureInfo lang)
        {
            var result = await _warehouseService.ImportAsync(file, lang);
            return (result != null ? result.Value : Ok());
        }
    }
}