using Cargotruck.Shared.Model;
using Cargotruck.Shared.Model.Dto;
using System.Globalization;

namespace Cargotruck.Server.Services
{
    public interface IMonthlyExpenseService
    {
        Task<List<Monthly_expensesDto>> GetAsync(int page, int pageSize, string sortOrder, bool desc, string? searchString, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate);
        Task<List<Monthly_expensesDto>> GetMonthlyExpensesAsync();
        Task<Monthly_expensesDto?> GetByIdAsync(int id);
        Task<int[]> GetChartDataAsync();
        Task<int> PageCountAsync(string? searchString, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate);
        Task<int> CountAsync();
        Task PostAsync(Monthly_expensesDto data);
        Task PutAsync(Monthly_expensesDto data);
        Task<bool> DeleteAsync(int id);
        Task CheckDataAsync();
        Task<List<Monthly_expenses_tasks_expensesDto>> GetConnectionIdsAsync();
        Task PostConnectionIdsAsync(Monthly_expenses_tasks_expensesDto connectionIds, bool first);
        Task CreateMonthsAsync();
        Task CreateConTableAsync();
        string ExportToExcel(CultureInfo lang, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate);
        Task<string> ExportToPdfAsync(CultureInfo lang, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate);
        Task<string> ExportToCSVAsync(CultureInfo lang, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate, bool isTextDocument);
    }
}