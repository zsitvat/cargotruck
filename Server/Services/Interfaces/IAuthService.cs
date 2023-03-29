using Cargotruck.Shared.Model.Dto;
using System.Globalization;

namespace Cargotruck.Server.Services.Interfaces
{
    public interface IAuthService
    {
        Task<string?> LoginAsync(LoginRequest request, CultureInfo lang);
        Task<string?> RegisterAsync(RegisterRequest parameters);
        Task<string?> UpdateAsync(UpdateRequest parameters);
        Task<string?> ChangePasswordAsync(ChangePasswordRequest parameters);
        Task LogoutAsync();
        CurrentUser CurrentUserInfo();
    }
}
