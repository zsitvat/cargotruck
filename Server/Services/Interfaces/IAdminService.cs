using Cargotruck.Shared.Model.Dto;

namespace Cargotruck.Server.Services.Interfaces
{
    public interface IAdminService
    {
        Task<List<UserDto>> GetAsync(int page, int pageSize, string? filter);
        Task<UserDto?> GetAsync(string id);
        Task<bool> DeleteAsync(string id);
        Task<int> PageCountAsync(string? filter);
        Task<int> CountAsync();
        Task<int> LoginsCountAsync();
        Task<Dictionary<string, string>?> ClaimsAsync();
        Task<Dictionary<string, string>?> RolesAsync();
    }
}