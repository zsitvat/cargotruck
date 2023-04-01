using Cargotruck.Shared.Model;
using Microsoft.AspNetCore.Mvc;

namespace Cargotruck.Server.Repositories.Interfaces
{
    public interface ISettingRepository
    {
        Task<List<Setting>> GetAsync();
        Task<Setting?> GetAsync(int id);
        Task<Setting> GetWaitTimeAsync();
        Task PostAsync(Setting data);
        Task PutAsync(Setting data);
        Task<bool> DeleteAsync(int id);
    }
}
