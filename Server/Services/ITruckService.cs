using Cargotruck.Shared.Model;
using Cargotruck.Shared.Model.Dto;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace Cargotruck.Server.Services
{
    public interface ITruckService
    {
        Task<List<TrucksDto>> GetAsync(int page, int pageSize, string sortOrder, bool desc, string? searchString, Status? filter, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate);
        Task<List<TrucksDto>> GetTrucksAsync();
        Task<TrucksDto?> GetByIdAsync(int id);
        Task<TrucksDto> GetByVRNAsync(string vehicle_registration_number);
        Task<int> PageCountAsync(string? searchString, Status? filter, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate);
        Task<int> CountAsync(bool all);
        Task PostAsync(TrucksDto data);
        Task PutAsync(TrucksDto data);
        Task<bool> DeleteAsync(int id);
        string ExportToExcel(CultureInfo lang, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate);
        Task<string> ExportToPdfAsync(CultureInfo lang, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate);
        Task<string> ExportToCSVAsync(CultureInfo lang, DateTime? dateFilterStartDate, DateTime? dateFilterEndDate, bool isTextDocument);
        Task<string?> ImportAsync([FromBody] string file, CultureInfo lang);
    }
}