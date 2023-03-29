using Cargotruck.Shared.Model.Dto;

namespace Cargotruck.Server.Services.Interfaces
{
    public interface IPrivacyService
    {
        Task<List<PrivaciesDto>> GetAsync(string lang);
        Task<PrivaciesDto?> GetByIdAsync(int id);
        Task<int> CountAsync();
        Task PostAsync(PrivaciesDto data);
        Task PutAsync(PrivaciesDto data);
        Task<bool> DeleteAsync(int id);
    }
}
