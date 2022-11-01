using Cargotruck.Shared.Models;

namespace Cargotruck.Client.Services
{
    public interface IAuthService
    {
        Task Login(LoginRequest loginRequest);
        Task Register(RegisterRequest registerRequest);
        Task Update(UpdateRequest UpdateRequest);
        Task ChangePassword(ChangePasswordRequest changePassword);
        Task Logout();
        Task<CurrentUser> CurrentUserInfo();
    }
}
