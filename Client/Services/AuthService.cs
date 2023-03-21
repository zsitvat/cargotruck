using Cargotruck.Shared.Models.Request;
using System.Globalization;
using System.Net.Http.Json;

namespace Cargotruck.Client.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;

        public AuthService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<CurrentUser> CurrentUserInfoAsync()
        {
            var result = await _httpClient.GetFromJsonAsync<CurrentUser>("api/auth/currentuserinfo");
            return result!;
        }

        public async Task LoginAsync(LoginRequest loginRequest)
        {
            var result = await _httpClient.PostAsJsonAsync($"api/auth/login?lang={CultureInfo.CurrentCulture}", loginRequest);
            if (result.StatusCode == System.Net.HttpStatusCode.BadRequest) throw new Exception(await result.Content.ReadAsStringAsync());
            result.EnsureSuccessStatusCode();
        }

        public async Task LogoutAsync()
        {
            var result = await _httpClient.PostAsync("api/auth/logout", null);
            result.EnsureSuccessStatusCode();
        }

        public async Task RegisterAsync(RegisterRequest registerRequest)
        {
            var result = await _httpClient.PostAsJsonAsync("api/auth/register", registerRequest);
            if (result.StatusCode == System.Net.HttpStatusCode.BadRequest) throw new Exception(await result.Content.ReadAsStringAsync());
            result.EnsureSuccessStatusCode();
        }
        public async Task UpdateAsync(UpdateRequest updateRequest)
        {
            var result = await _httpClient.PostAsJsonAsync("api/auth/update", updateRequest);
            if (result.StatusCode == System.Net.HttpStatusCode.BadRequest) throw new Exception(await result.Content.ReadAsStringAsync());
            result.EnsureSuccessStatusCode();
        }

        public async Task ChangePasswordAsync(ChangePasswordRequest changePassword)
        {
            var result = await _httpClient.PostAsJsonAsync("api/auth/changepassword", changePassword);
            if (result.StatusCode == System.Net.HttpStatusCode.BadRequest) throw new Exception(await result.Content.ReadAsStringAsync());
            result.EnsureSuccessStatusCode();
        }
    }
}
