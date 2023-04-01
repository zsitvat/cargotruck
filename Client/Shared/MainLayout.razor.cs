using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.Authorization;
using Cargotruck.Shared.Model;
using Cargotruck.Client.Services;
using Cargotruck.Shared.Model.Dto;

namespace Cargotruck.Client.Shared
{
    public partial class MainLayout
    {
        bool showError = false;
        bool expandSubMenu;
        string currency_api_error = "";
        [CascadingParameter]
        Task<AuthenticationState>? AuthenticationState { get; set; }

        static bool darkmode;
        protected override async Task OnInitializedAsync()
        {
            await OnParametersSetAsync();
        }

        protected override async Task OnParametersSetAsync()
        {
            if (!(await AuthenticationState!).User.Identity!.IsAuthenticated)
            {
                if (!navigationManager.Uri.ToString().Contains("login") && !navigationManager.Uri.ToString().Contains("Privacy"))
                {
                    navigationManager.NavigateTo("/login");
                }
            }
            else
            {
                await GetDarkmodeAsync();
            }
            darkmode = await sessionStorage.GetItemAsync<bool>("darkmode");

            if ((await AuthenticationState!).User.Identity!.IsAuthenticated)
            {
                await GetCurrencyRates();
            }
        }

        async Task GetCurrencyRates()
        {
            if ((await AuthenticationState!).User.Identity!.IsAuthenticated && currencyExchange.GetRates() == null && await currencyExchange.GetNextApiRequestDate(client) <= DateTime.Now)
            {
                try
                {
                    await currencyExchange.RequestRatesFromApiAsync(client);
                }
                catch (Exception ex)
                {
                    currency_api_error = $"Error - Type: {ex.GetType()}, Message: {ex.Message}";
                    if (ex.GetType().ToString() == "Microsoft.CSharp.RuntimeBinder.RuntimeBinderException")
                    {
                        currency_api_error = "currency_api_is_exceeded";
                    }
                }
            }
        }
      

        async Task GetDarkmodeAsync()
        {
            var settings = await client.GetFromJsonAsync<Settings[]>("api/settings/get");
            var DarkModeSetting = (settings?.Where(x => x.SettingName == "darkmode" && x.SettingValue == (AuthenticationState!).Result.User.Identity?.Name));
            var darkModeFound = (DarkModeSetting?.Count() > 0 ? true : false);
            
            await sessionStorage.SetItemAsync("darkmode", darkModeFound);
        }

        async Task ChangeDarkModeAsync()
        {
            if ((await AuthenticationState!).User.Identity!.IsAuthenticated)
            {
                var settings = await client.GetFromJsonAsync<Settings[]>("api/settings/get");
                var darkModeSetting = (settings?.FirstOrDefault(x => x.SettingName == "darkmode" && x.SettingValue == (AuthenticationState!).Result.User.Identity?.Name));
                
                if (darkModeSetting != null)
                {
                   
                    await client.DeleteAsync($"api/settings/delete/{darkModeSetting?.Id}");
                    await sessionStorage.SetItemAsync("darkmode", false);
                }
                else
                {
                    SettingsDto setting = new()
                    {
                        SettingValue = (AuthenticationState!).Result.User.Identity?.Name,
                        SettingName = "darkmode"
                    };
                    await client.PostAsJsonAsync("api/settings/post", setting);
                    await sessionStorage.SetItemAsync("darkmode", await sessionStorage.GetItemAsync<bool>("darkmode"));
                }
            }
            else
            {
                await sessionStorage.SetItemAsync("darkmode", !await sessionStorage.GetItemAsync<bool>("darkmode"));
            }

            await OnParametersSetAsync();
        }

        async Task LogoutClick()
        {
            PageHistoryState.ResetPageToHistory();
            await authStateProvider.LogoutAsync();
            navigationManager.NavigateTo("/login");
        }

    }
}