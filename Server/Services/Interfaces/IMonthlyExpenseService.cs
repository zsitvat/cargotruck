using Cargotruck.Shared.Model;
using Cargotruck.Shared.Model.Dto;
using System.Globalization;

namespace Cargotruck.Server.Services.Interfaces
{
    public interface IMonthlyExpenseService
    {
        Task<List<MonthlyExpenseDto>> GetAsync(int page, int pageSize, string sortOrder, bool desc, string? searchString, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate);
        Task<List<MonthlyExpenseDto>> GetMonthlyExpensesAsync();
        Task<MonthlyExpenseDto?> GetByIdAsync(int id);
        Task<int[]> GetChartDataAsync();
        Task<int> PageCountAsync(string? searchString, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate);
        Task<int> CountAsync();
        Task PostAsync(MonthlyExpenseDto data);
        Task PutAsync(MonthlyExpenseDto data);
        Task<bool> DeleteAsync(int id);
        Task CheckDataAsync();
        Task<List<MonthlyExpense_task_expenseDto>> GetConnectionIdsAsync();
        Task PostConnectionIdsAsync(MonthlyExpense_task_expenseDto connectionIds, bool first);
        Task CreateMonthsAsync();
        Task CreateConTableAsync();
        string ExportToExcel(CultureInfo lang, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate);
        Task<string> ExportToPdfAsync(CultureInfo lang, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate);
        Task<string> ExportToCSVAsync(CultureInfo lang, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate, bool isTextDocument);
    }
}