using Cargotruck.Shared.Model;
using Cargotruck.Shared.Model.Dto;
using System.Globalization;

namespace Cargotruck.Server.Services.Interfaces
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
        Task<List<Monthly_expense_task_expenseDto>> GetConnectionIdsAsync();
        Task PostConnectionIdsAsync(Monthly_expense_task_expenseDto connectionIds, bool first);
        Task CreateMonthsAsync();
        Task CreateConTableAsync();
        string ExportToExcel(CultureInfo lang, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate);
        Task<string> ExportToPdfAsync(CultureInfo lang, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate);
        Task<string> ExportToCSVAsync(CultureInfo lang, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate, bool isTextDocument);
    }
}