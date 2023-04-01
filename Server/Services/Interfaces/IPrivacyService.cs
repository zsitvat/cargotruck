using Cargotruck.Shared.Model.Dto;

namespace Cargotruck.Server.Services.Interfaces
{
    public interface IPrivacyService
    {
        Task<List<PrivacyDto>> GetAsync(string lang);
        Task<PrivacyDto?> GetByIdAsync(int id);
        Task<int> CountAsync();
        Task PostAsync(PrivacyDto data);
        Task PutAsync(PrivacyDto data);
        Task<bool> DeleteAsync(int id);
    }
}
