using Cargotruck.Shared.Model.Dto;

namespace Cargotruck.Server.Services.Interfaces
{
    public interface ISettingService
    {
        Task<List<SettingDto>> GetAsync();
        Task<SettingDto?> GetAsync(int id);
        Task<SettingDto> GetWaitTimeAsync();
        Task PostAsync(SettingDto data);
        Task PutAsync(SettingDto data);
        Task<bool> DeleteAsync(int id);
    }
}