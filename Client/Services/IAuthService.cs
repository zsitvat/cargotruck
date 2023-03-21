using Cargotruck.Shared.Models.Request;

namespace Cargotruck.Client.Services
{
    public interface IAuthService
    {
        Task LoginAsync(LoginRequest loginRequest);
        Task RegisterAsync(RegisterRequest registerRequest);
        Task UpdateAsync(UpdateRequest UpdateRequest);
        Task ChangePasswordAsync(ChangePasswordRequest changePassword);
        Task LogoutAsync();
        Task<CurrentUser> CurrentUserInfoAsync();
    }
}
