using Cargotruck.Shared.Model.Dto;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace Cargotruck.Server.Services
{
    public interface IWarehouseService
    {
        Task<List<WarehousesDto>> GetAsync(int page, int pageSize, string sortOrder, bool desc, string? searchString, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate);
        Task<List<WarehousesDto>> GetWarehousesAsync();
        Task<WarehousesDto?> GetByIdAsync(int id);
        Task<int> PageCountAsync(string? searchString, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate);
        Task<int> CountAsync(bool all);
        Task PostAsync(WarehousesDto data);
        Task PutAsync(WarehousesDto data);
        Task<bool> DeleteAsync(int id);
        string ExportToExcel(CultureInfo lang, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate);
        Task<string> ExportToPdfAsync(CultureInfo lang, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate);
        Task<string> ExportToCSVAsync(CultureInfo lang, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate, bool isTextDocument);
        Task<ActionResult<string?>> ImportAsync([FromBody] string file, CultureInfo lang);
    }
}
