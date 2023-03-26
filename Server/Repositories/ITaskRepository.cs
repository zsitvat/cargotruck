using Cargotruck.Shared.Model;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace Cargotruck.Server.Repositories
{
    public interface ITaskRepository
    {
        Task<List<Tasks>> GetAsync(int page, int pageSize, string sortOrder, bool desc, string? searchString, string? filter, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate);
        Task<List<Tasks>> GetTasksAsync();
        Task<Tasks?> GetByIdAsync(int id);
        Task<int[]> GetChartDataAsync();
        Task<int> PageCountAsync(string? searchString, string? filter, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate);
        Task<int> CountAsync(bool all);
        Task ChangeCompletionAsync(Tasks t);
        Task PostAsync(Tasks t);
        Task PutAsync(Tasks t);
        Task<bool> DeleteAsync(int id);     
        Task<string> ExportToCSVAsync(CultureInfo lang, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate, bool isTextDocument);
        string ExportToExcel(CultureInfo lang, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate);
        Task<string> ExportToPdfAsync(CultureInfo lang, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate);
        Task<string> ImportAsync([FromBody] string file, CultureInfo lang);
    }
}