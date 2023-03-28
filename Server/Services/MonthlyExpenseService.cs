using AutoMapper;
using Cargotruck.Server.Repositories;
using Cargotruck.Shared.Model;
using Cargotruck.Shared.Model.Dto;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace Cargotruck.Server.Services
{
    public class MonthlyExpenseService : IMonthlyExpenseService
    {
        private readonly IMonthlyExpenseRepository _monthlyExpenseRepository;
        private readonly IMapper _mapper;
        public MonthlyExpenseService(IMonthlyExpenseRepository monthlyExpenseRepository, IMapper mapper)
        {
            _monthlyExpenseRepository = monthlyExpenseRepository;
            _mapper = mapper;
        }
        public async Task<List<Monthly_expensesDto>> GetAsync(int page, int pageSize, string sortOrder, bool desc, string? searchString, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate)
        {
            var tasks = await _monthlyExpenseRepository.GetAsync(page, pageSize, sortOrder, desc, searchString, dateFilterStartDate, dateFilterEndDate);
            return _mapper.Map<List<Monthly_expensesDto>>(tasks);
        }
        public async Task<List<Monthly_expensesDto>> GetMonthlyExpensesAsync()
        {
            var monthlyExpenses = await _monthlyExpenseRepository.GetMonthlyExpensesAsync();

            return _mapper.Map<List<Monthly_expensesDto>>(monthlyExpenses);
        }
        public async Task<Monthly_expensesDto?> GetByIdAsync(int id)
        {
            var monthlyExpense = await _monthlyExpenseRepository.GetByIdAsync(id);

            return _mapper.Map<Monthly_expensesDto>(monthlyExpense);
        }
        public async Task<int[]> GetChartDataAsync()
        {
            return await _monthlyExpenseRepository.GetChartDataAsync();
        }
        public async Task<int> PageCountAsync(string? searchString,  DateTime? dateFilterStartDate, DateTime? dateFilterEndDate)
        {
            return await _monthlyExpenseRepository.PageCountAsync(searchString,  dateFilterStartDate, dateFilterEndDate);
        }
        public async Task<int> CountAsync()
        {
            return await _monthlyExpenseRepository.CountAsync();
        }
        public async Task PostAsync(Monthly_expensesDto task)
        {
            await _monthlyExpenseRepository.PostAsync(_mapper.Map<Monthly_expenses>(task));
        }
        public async Task PutAsync(Monthly_expensesDto task)
        {
            await _monthlyExpenseRepository.PutAsync(_mapper.Map<Monthly_expenses>(task));
        }
        public async Task<bool> DeleteAsync(int id)
        {
            return await _monthlyExpenseRepository.DeleteAsync(id);
        }
        public async Task CheckDataAsync()
        {
            await _monthlyExpenseRepository.CheckDataAsync();
        }
        public async Task<List<Monthly_expenses_tasks_expensesDto>> GetConnectionIdsAsync()
        {
            var monthlyExpensesConIds =  await _monthlyExpenseRepository.GetConnectionIdsAsync();

            return _mapper.Map<List<Monthly_expenses_tasks_expensesDto>>(monthlyExpensesConIds);
        }

        public async Task PostConnectionIdsAsync(Monthly_expenses_tasks_expensesDto connectionIds, bool first)
        {
            await _monthlyExpenseRepository.PostConnectionIdsAsync(_mapper.Map<Monthly_expenses_tasks_expenses>(connectionIds), first);
        }
        public async Task CreateMonthsAsync()
        {
            await _monthlyExpenseRepository.CreateMonthsAsync();
        }
        public async Task CreateConTableAsync()
        {
            await _monthlyExpenseRepository.CreateConTableAsync();
        }
        public async Task<string> ExportToCSVAsync(CultureInfo lang, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate, bool isTextDocument)
        {
            return await _monthlyExpenseRepository.ExportToCSVAsync(lang, dateFilterStartDate, dateFilterEndDate, isTextDocument);
        }
        public string ExportToExcel(CultureInfo lang, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate)
        {
            return _monthlyExpenseRepository.ExportToExcel(lang, dateFilterStartDate, dateFilterEndDate);
        }
        public async Task<string> ExportToPdfAsync(CultureInfo lang, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate)
        {
            return await _monthlyExpenseRepository.ExportToPdfAsync(lang, dateFilterStartDate, dateFilterEndDate);
        }
    }
}
