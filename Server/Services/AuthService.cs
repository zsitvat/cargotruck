using AutoMapper;
using Cargotruck.Server.Models;
using Cargotruck.Server.Repositories.Interfaces;
using Cargotruck.Server.Services.Interfaces;
using Cargotruck.Shared.Model.Dto;
using System.Globalization;

namespace Cargotruck.Server.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _AuthRepository;
        public AuthService(IAuthRepository AuthRepository)
        {
            _AuthRepository = AuthRepository;
        }
        public async Task<string?> LoginAsync(LoginRequest request, CultureInfo lang) 
        { 
            return await _AuthRepository.LoginAsync(request, lang);
        }
        public async Task<string?> RegisterAsync(RegisterRequest parameters)
        {
            return await _AuthRepository.RegisterAsync(parameters);   
        }
        public async Task<string?> UpdateAsync(UpdateRequest parameters)
        {
            return await  _AuthRepository.UpdateAsync(parameters);   
        }
        public async Task<string?> ChangePasswordAsync(ChangePasswordRequest parameters)
        {
            return await  _AuthRepository.ChangePasswordAsync(parameters);
        }
        public async Task LogoutAsync()
        {
              await _AuthRepository.LogoutAsync();
        }
        public CurrentUser CurrentUserInfo()
        {
            return _AuthRepository.CurrentUserInfo();
        }
    }
}
