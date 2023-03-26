using Cargotruck.Shared.Model.Dto;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace Cargotruck.Server.Services
{
    public interface ITaskService
    {
        Task ChangeCompletionAsync(TasksDto task);
        Task<int> CountAsync(bool all);
        Task<bool> DeleteAsync(int id);
        Task<string> ExportToCSVAsync(CultureInfo lang, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate, bool isTextDocument);
        Task<string> ExportToPdfAsync(CultureInfo lang, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate);
        string ExportToExcel(CultureInfo lang, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate);
        Task<List<TasksDto>> GetAsync(int page, int pageSize, string sortOrder, bool desc, string? searchString, string? filter, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate);
        Task<TasksDto?> GetByIdAsync(int id);
        Task<int[]> GetChartDataAsync();
        Task<List<TasksDto>> GetTasksAsync();
        Task<ActionResult<string>> ImportAsync([FromBody] string file, CultureInfo lang);
        Task<int> PageCountAsync(string? searchString, string? filter, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate);
        Task PostAsync(TasksDto task);
        Task PutAsync(TasksDto task);
    }
}