using Cargotruck.Shared.Model;
using Microsoft.AspNetCore.Mvc;

namespace Cargotruck.Server.Repositories
{
    public interface ISettingRepository
    {
        Task<List<Settings>> GetAsync();
        Task<Settings?> GetAsync(int id);
        Task<Settings> GetWaitTimeAsync();
        Task PostAsync(Settings data);
        Task PutAsync(Settings data);
        Task<bool> DeleteAsync(int id);
    }
}
