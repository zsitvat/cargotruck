using AutoMapper;
using Cargotruck.Server.Repositories;
using Cargotruck.Shared.Model;
using Cargotruck.Shared.Model.Dto;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace Cargotruck.Server.Services
{
    public class ExpenseService : IExpenseService
    {
        private readonly IExpenseRepository _expenseRepository;
        private readonly IMapper _mapper;
        public ExpenseService(IExpenseRepository expenseRepository, IMapper mapper)
        {
            _expenseRepository = expenseRepository;
            _mapper = mapper;
        }
        public async Task<List<ExpensesDto>> GetAsync(int page, int pageSize, string sortOrder, bool desc, string? searchString, string? filter, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate)
        {
            var expenses = await _expenseRepository.GetAsync(page, pageSize, sortOrder, desc, searchString, filter, dateFilterStartDate, dateFilterEndDate);
            return _mapper.Map<List<ExpensesDto>>(expenses);
        }
        public async Task<List<ExpensesDto>> GetExpensesAsync()
        {
            var expenses = await _expenseRepository.GetExpensesAsync();

            return _mapper.Map<List<ExpensesDto>>(expenses);
        }
        public async Task<ExpensesDto?> GetByIdAsync(int id)
        {
            var expense = await _expenseRepository.GetByIdAsync(id);

            return _mapper.Map<ExpensesDto>(expense);
        }
        public async Task<int> PageCountAsync(string? searchString, string? filter, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate)
        {
            var count = await _expenseRepository.PageCountAsync(searchString, filter, dateFilterStartDate, dateFilterEndDate);

            return count;
        }
        public async Task<int> CountAsync()
        {
            return await _expenseRepository.CountAsync();
        }
        public async Task PostAsync(ExpensesDto expense)
        {
            await _expenseRepository.PostAsync(_mapper.Map<Expenses>(expense));
        }
        public async Task PutAsync(ExpensesDto expense)
        {
            await _expenseRepository.PutAsync(_mapper.Map<Expenses>(expense));
        }
        public async Task<bool> DeleteAsync(int id)
        {
            return await _expenseRepository.DeleteAsync(id);
        }
        public async Task<string> ExportToCSVAsync(CultureInfo lang, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate, bool isTextDocument)
        {
            return await _expenseRepository.ExportToCSVAsync(lang, dateFilterStartDate, dateFilterEndDate, isTextDocument);
        }
        public string ExportToExcel(CultureInfo lang, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate)
        {
            return _expenseRepository.ExportToExcel(lang, dateFilterStartDate, dateFilterEndDate);
        }
        public async Task<string> ExportToPdfAsync(CultureInfo lang, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate)
        {
            return await _expenseRepository.ExportToPdfAsync(lang, dateFilterStartDate, dateFilterEndDate);
        }
        public async Task<string?> ImportAsync([FromBody] string file, CultureInfo lang)
        {
            return await _expenseRepository.ImportAsync(file, lang);
        }
    }
}
