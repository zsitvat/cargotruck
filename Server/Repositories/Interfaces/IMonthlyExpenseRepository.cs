using Cargotruck.Shared.Model;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace Cargotruck.Server.Repositories.Interfaces
{
    public interface IMonthlyExpenseRepository
    {
        Task<List<MonthlyExpense>> GetAsync(int page, int pageSize, string sortOrder, bool desc, string? searchString, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate);
        Task<List<MonthlyExpense>> GetMonthlyExpensesAsync();
        Task<MonthlyExpense?> GetByIdAsync(int id);
        Task<int[]> GetChartDataAsync();
        Task<int> PageCountAsync(string? searchString, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate);
        Task<int> CountAsync();
        Task PostAsync(MonthlyExpense data);
        Task PutAsync(MonthlyExpense data);
        Task<bool> DeleteAsync(int id);
        Task CheckDataAsync();
        Task<List<MonthlyExpense_task_expense>> GetConnectionIdsAsync();
        Task PostConnectionIdsAsync(MonthlyExpense_task_expense connectionIds, bool first);
        Task CreateMonthsAsync();
        Task CreateConTableAsync();
        string ExportToExcel(CultureInfo lang, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate);
        Task<string> ExportToPdfAsync(CultureInfo lang, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate);
        Task<string> ExportToCSVAsync(CultureInfo lang, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate, bool isTextDocument);
    }
}