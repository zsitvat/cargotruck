using Cargotruck.Shared.Model.Dto;

namespace Cargotruck.Server.Services.Interfaces
{
    public interface ISettingService
    {
        Task<List<SettingsDto>> GetAsync();
        Task<SettingsDto?> GetAsync(int id);
        Task<SettingsDto> GetWaitTimeAsync();
        Task PostAsync(SettingsDto data);
        Task PutAsync(SettingsDto data);
        Task<bool> DeleteAsync(int id);
    }
}