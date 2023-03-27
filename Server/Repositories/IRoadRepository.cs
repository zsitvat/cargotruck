using Cargotruck.Shared.Model;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace Cargotruck.Server.Repositories
{
    public interface IRoadRepository
    {
        Task<List<Roads>> GetAsync(int page, int pageSize, string sortOrder, bool desc, string? searchString, string? filter, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate);
        Task<List<Roads>> GetRoadsAsync();
        Task<Roads?> GetByIdAsync(int id);
        Task<int[]> GetChartDataAsync();
        Task<int> PageCountAsync(string? searchString, string? filter, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate);
        Task<int> CountAsync();
        Task PostAsync(Roads t);
        Task PutAsync(Roads t);
        Task<bool> DeleteAsync(int id);
        string ExportToExcel(CultureInfo lang, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate);
        Task<string> ExportToPdfAsync(CultureInfo lang, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate);
        Task<string> ExportToCSVAsync(CultureInfo lang, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate, bool isTextDocument);
        Task<string?> ImportAsync([FromBody] string file, CultureInfo lang);
    }
}