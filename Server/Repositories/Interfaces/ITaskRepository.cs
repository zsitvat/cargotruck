using Cargotruck.Shared.Model;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace Cargotruck.Server.Repositories.Interfaces
{
    public interface ITaskRepository
    {
        Task<List<DeliveryTask>> GetAsync(int page, int pageSize, string sortOrder, bool desc, string? searchString, string? filter, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate);
        Task<List<DeliveryTask>> GetTasksAsync();
        Task<DeliveryTask?> GetByIdAsync(int id);
        Task<int[]> GetChartDataAsync();
        Task<int> PageCountAsync(string? searchString, string? filter, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate);
        Task<int> CountAsync(bool all);
        Task ChangeCompletionAsync(DeliveryTask t);
        Task PostAsync(DeliveryTask t);
        Task PutAsync(DeliveryTask t);
        Task<bool> DeleteAsync(int id);
        string ExportToExcel(CultureInfo lang, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate);
        Task<string> ExportToPdfAsync(CultureInfo lang, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate);
        Task<string> ExportToCSVAsync(CultureInfo lang, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate, bool isTextDocument);
        Task<string?> ImportAsync([FromBody] string file, CultureInfo lang);
    }
}