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
    public class ExpensesController : ControllerBase
    {
        private readonly IExpenseService _expenseService;

        public ExpensesController(IExpenseService expenseService)
        {
            _expenseService = expenseService;
        }

        [HttpGet]
        public async Task<ActionResult<List<ExpenseDto>>> GetAsync(int page, int pageSize, string sortOrder,
            bool desc, string? searchString, string? filter, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate)
        {
            return Ok(await _expenseService.GetAsync(page, pageSize, sortOrder, desc, searchString, filter, dateFilterStartDate, dateFilterEndDate));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ExpenseDto>> GetByIdAsync(int id)
        {
            return Ok(await _expenseService.GetByIdAsync(id));
        }

        [HttpGet]
        public async Task<ActionResult<List<ExpenseDto>>> GetExpensesAsync()
        {
            return Ok(await _expenseService.GetExpensesAsync());
        }

        [HttpGet]
        public async Task<ActionResult<int>> CountAsync()
        {
            return Ok(await _expenseService.CountAsync());
        }

        [HttpGet]
        public async Task<ActionResult<int>> PageCountAsync(string? searchString, string? filter, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate)
        {
            return Ok(await _expenseService.PageCountAsync(searchString, filter, dateFilterStartDate, dateFilterEndDate));
        }

        [HttpPost]
        public async Task PostAsync(ExpenseDto task)
        {
            await _expenseService.PostAsync(task);
        }

        [HttpPut]
        public async Task PutAsync(ExpenseDto task)
        {
            await _expenseService.PutAsync(task);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> DeleteAsync(int id)
        {
            return Ok(await _expenseService.DeleteAsync(id));
        }

        //closedXML needed !!!
        [HttpGet]
        public string ExportToExcel(CultureInfo lang, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate)
        {
            return _expenseService.ExportToExcel(lang, dateFilterStartDate, dateFilterEndDate);
        }

        //iTextSharp needed !!!
        [HttpGet]
        public async Task<string> ExportToPdfAsync(CultureInfo lang, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate)
        {
            return await _expenseService.ExportToPdfAsync(lang, dateFilterStartDate, dateFilterEndDate);
        }

        [HttpGet]
        public async Task<string> ExportToCSVAsync(CultureInfo lang, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate, bool isTextDocument)
        {
            return await _expenseService.ExportToCSVAsync(lang, dateFilterStartDate, dateFilterEndDate, isTextDocument);
        }


        [HttpPost]
        public async Task<ActionResult<string?>> ImportAsync([FromBody] string file, CultureInfo lang)
        {
            var result = await _expenseService.ImportAsync(file, lang);
            return (!string.IsNullOrEmpty(result) ? BadRequest(result) : Ok());
        }
    }
}