using Cargotruck.Shared.Model.Dto;

namespace Cargotruck.Client.Services.Interfaces
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
