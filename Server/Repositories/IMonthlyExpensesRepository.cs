using Cargotruck.Shared.Model;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace Cargotruck.Server.Repositories
{
    public interface IMonthlyExpensesRepository
    {
        Task<List<Monthly_expenses>> GetAsync(int page, int pageSize, string sortOrder, bool desc, string? searchString, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate);
        Task<List<Monthly_expenses>> GetMonthlyExpensesAsync();
        Task<Monthly_expenses?> GetByIdAsync(int id);
        Task<int[]> GetChartDataAsync();
        Task<int> PageCountAsync(string? searchString, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate);
        Task<int> CountAsync();
        Task PostAsync(Monthly_expenses data);
        Task PutAsync(Monthly_expenses data); 
        Task<bool> DeleteAsync(int id);
        Task CheckDataAsync();
        Task PostConnectionIdsAsync(Monthly_expenses_tasks_expenses connectionIds, bool first);
        Task CreateMonthsAsync();
        Task CreateConTableAsync();
        string ExportToExcel(CultureInfo lang, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate);
        Task<string> ExportToPdfAsync(CultureInfo lang, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate);
        Task<string> ExportToCSVAsync(CultureInfo lang, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate, bool isTextDocument);
    }
}