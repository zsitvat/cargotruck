﻿using Cargotruck.Server.Services.Interfaces;
using Cargotruck.Shared.Model.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace Cargotruck.Server.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class MonthlyExpensesController : ControllerBase
    {
        private readonly IMonthlyExpenseService _monthlyExpenseService;

        public MonthlyExpensesController(IMonthlyExpenseService monthlyExpenseService)
        {
            _monthlyExpenseService = monthlyExpenseService;
        }

        [HttpGet]
        public async Task<ActionResult<List<Monthly_expensesDto>>> GetAsync(int page, int pageSize, string sortOrder,
            bool desc, string? searchString, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate)
        {
            return Ok(await _monthlyExpenseService.GetAsync(page, pageSize, sortOrder, desc, searchString, dateFilterStartDate, dateFilterEndDate));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Monthly_expensesDto>> GetByIdAsync(int id)
        {
            return Ok(await _monthlyExpenseService.GetByIdAsync(id));
        }

        [HttpGet]
        public async Task<ActionResult<List<Monthly_expensesDto>>> GetMonthlyExpensesAsync()
        {
            return Ok(await _monthlyExpenseService.GetMonthlyExpensesAsync());
        }

        [HttpGet]
        public async Task<ActionResult<int[]>> GetChartDataAsync()
        {
            return Ok(await _monthlyExpenseService.GetChartDataAsync());
        }

        [HttpGet]
        public async Task<ActionResult<int>> CountAsync()
        {
            return Ok(await _monthlyExpenseService.CountAsync());
        }

        [HttpGet]
        public async Task<ActionResult<int>> PageCountAsync(string? searchString, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate)
        {
            return Ok(await _monthlyExpenseService.PageCountAsync(searchString,  dateFilterStartDate, dateFilterEndDate));
        }

        [HttpPost]
        public async Task PostAsync(Monthly_expensesDto task)
        {
            await _monthlyExpenseService.PostAsync(task);
        }

        [HttpPut]
        public async Task PutAsync(Monthly_expensesDto task)
        {
            await _monthlyExpenseService.PutAsync(task);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> DeleteAsync(int id)
        {
            return Ok(await _monthlyExpenseService.DeleteAsync(id));
        }

        [HttpPost]
        public async Task CheckDataAsync()
        {
            await _monthlyExpenseService.CheckDataAsync();
        }

        [HttpGet]
        public async Task<List<Monthly_expenses_tasks_expensesDto>> GetConnectionIdsAsync()
        {
            return await _monthlyExpenseService.GetConnectionIdsAsync();
        }

        [HttpPost]
        public async Task PostConnectionIdsAsync(Monthly_expenses_tasks_expensesDto connectionIds, bool first)
        {
            await _monthlyExpenseService.PostConnectionIdsAsync(connectionIds, first);
        }

        [HttpPost]
        public async Task CreateMonthsAsync()
        {
            await _monthlyExpenseService.CreateMonthsAsync();
        }

        [HttpPost]
        public async Task CreateConTableAsync()
        {
            await _monthlyExpenseService.CreateConTableAsync();
        }

        //closedXML needed !!!
        [HttpGet]
        public string ExportToExcel(CultureInfo lang, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate)
        {
            return _monthlyExpenseService.ExportToExcel(lang, dateFilterStartDate, dateFilterEndDate);
        }

        //iTextSharp needed !!!
        [HttpGet]
        public async Task<string> ExportToPdfAsync(CultureInfo lang, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate)
        {
            return await _monthlyExpenseService.ExportToPdfAsync(lang, dateFilterStartDate, dateFilterEndDate);
        }

        [HttpGet]
        public async Task<string> ExportToCSVAsync(CultureInfo lang, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate, bool isTextDocument)
        {
            return await _monthlyExpenseService.ExportToCSVAsync(lang, dateFilterStartDate, dateFilterEndDate, isTextDocument);
        }

    }
}