using Cargotruck.Shared.Model;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace Cargotruck.Server.Repositories.Interfaces
{
    public interface IMonthlyExpenseRepository
    {
        Task<List<Monthly_expense>> GetAsync(int page, int pageSize, string sortOrder, bool desc, string? searchString, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate);
        Task<List<Monthly_expense>> GetMonthlyExpensesAsync();
        Task<Monthly_expense?> GetByIdAsync(int id);
        Task<int[]> GetChartDataAsync();
        Task<int> PageCountAsync(string? searchString, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate);
        Task<int> CountAsync();
        Task PostAsync(Monthly_expense data);
        Task PutAsync(Monthly_expense data);
        Task<bool> DeleteAsync(int id);
        Task CheckDataAsync();
        Task<List<Monthly_expense_task_expense>> GetConnectionIdsAsync();
        Task PostConnectionIdsAsync(Monthly_expense_task_expense connectionIds, bool first);
        Task CreateMonthsAsync();
        Task CreateConTableAsync();
        string ExportToExcel(CultureInfo lang, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate);
        Task<string> ExportToPdfAsync(CultureInfo lang, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate);
        Task<string> ExportToCSVAsync(CultureInfo lang, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate, bool isTextDocument);
    }
}