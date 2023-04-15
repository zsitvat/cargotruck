using Cargotruck.Client.Services.Interfaces;
using Cargotruck.Shared.Model.Dto;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace Cargotruck.Client.Services
{
    public class CustomStateProvider : AuthenticationStateProvider
    {
        private readonly IAuthService api;
        private CurrentUser? _currentUser;
        public CustomStateProvider(IAuthService api)
        {
            this.api = api;
        }
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var identity = new ClaimsIdentity();
            try
            {
                var userInfo = await GetCurrentUserAsync();
                if (userInfo.IsAuthenticated && _currentUser?.UserName != null)
                {
                    var claims = new[] { new Claim(ClaimTypes.Name, _currentUser.UserName) }.Concat(_currentUser.Claims.Select(c => new Claim(c.Key, c.Value)));
                    identity = new ClaimsIdentity(claims, "Server authentication");
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine("Dto failed:" + ex.ToString());
            }

            return new AuthenticationState(new ClaimsPrincipal(identity));
        }

        public async Task<CurrentUser> GetCurrentUserAsync()
        {
            if (_currentUser != null && _currentUser.IsAuthenticated)
            {
                return _currentUser;
            }
                
            _currentUser = await api.CurrentUserInfoAsync();
            return _currentUser;
        }
        public async Task LogoutAsync()
        {
            await api.LogoutAsync();
            _currentUser = null;
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }
        public async Task LoginAsync(LoginRequest loginParameters)
        {
            await api.LoginAsync(loginParameters);
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }
        public async Task RegisterAsync(RegisterRequest registerParameters)
        {
            await api.RegisterAsync(registerParameters);
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }
        public async Task UpdateAsync(UpdateRequest updateParameters)
        {
            await api.UpdateAsync(updateParameters);
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }
        public async Task ChangePasswordAsync(ChangePasswordRequest updateParameters)
        {
            await api.ChangePasswordAsync(updateParameters);
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }
    }
}
