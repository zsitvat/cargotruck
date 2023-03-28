using Cargotruck.Shared.Model;
using Microsoft.AspNetCore.Mvc;

namespace Cargotruck.Server.Repositories
{
    public interface IPrivacyRepository
    {
        Task<List<Privacies>> GetAsync(string lang);
        Task<Privacies?> GetByIdAsync(int id);
        Task<int> CountAsync();
        Task PostAsync(Privacies data);
        Task PutAsync(Privacies data);
        Task<bool> DeleteAsync(int id);
    }
}