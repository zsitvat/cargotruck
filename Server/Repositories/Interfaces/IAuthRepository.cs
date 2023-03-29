using Cargotruck.Shared.Model.Dto;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Security.Claims;

namespace Cargotruck.Server.Repositories.Interfaces
{
    public interface IAuthRepository
    {
        Task<string?> LoginAsync(LoginRequest request, CultureInfo lang);
        Task<string?> RegisterAsync(RegisterRequest parameters);
        Task<string?> UpdateAsync(UpdateRequest parameters);
        Task<string?> ChangePasswordAsync(ChangePasswordRequest parameters);
        Task LogoutAsync();
        CurrentUser CurrentUserInfo();
    }
}