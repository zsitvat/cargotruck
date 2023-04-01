using Cargotruck.Shared.Model.Dto;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace Cargotruck.Server.Services.Interfaces
{
    public interface ITaskService
    {
        Task<List<DeliveryTaskDto>> GetAsync(int page, int pageSize, string sortOrder, bool desc, string? searchString, string? filter, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate);
        Task<List<DeliveryTaskDto>> GetTasksAsync();
        Task<DeliveryTaskDto?> GetByIdAsync(int id);
        Task<int[]> GetChartDataAsync();
        Task<int> PageCountAsync(string? searchString, string? filter, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate);
        Task<int> CountAsync(bool all);
        Task ChangeCompletionAsync(DeliveryTaskDto task);
        Task PostAsync(DeliveryTaskDto task);
        Task PutAsync(DeliveryTaskDto task);
        Task<bool> DeleteAsync(int id);
        string ExportToExcel(CultureInfo lang, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate);
        Task<string> ExportToPdfAsync(CultureInfo lang, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate);
        Task<string> ExportToCSVAsync(CultureInfo lang, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate, bool isTextDocument);
        Task<string?> ImportAsync([FromBody] string file, CultureInfo lang);
    }
}