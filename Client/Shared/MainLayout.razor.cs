using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.Authorization;
using Cargotruck.Shared.Models;
using Cargotruck.Client.UtilitiesClasses;

namespace Cargotruck.Client.Shared
{
    public partial class MainLayout
    {
        bool showError = false;
        bool expandSubMenu;
        string? currency_api_error;
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
                darkmode = await sessionStorage.GetItemAsync<bool>("darkmode");
                if (!navigationManager.Uri.ToString().Contains("login") && !navigationManager.Uri.ToString().Contains("Privacy"))
                {
                    navigationManager.NavigateTo("/login");
                }
            }
            else
            {
                var settings = await client.GetFromJsonAsync<Settings[]>("api/settings/get");
                var DarkModeSetting = (settings?.Where(x => x.SettingName == "darkmode" && x.SettingValue == (AuthenticationState!).Result.User.Identity?.Name));
                var darkModeFound = (DarkModeSetting?.Count() > 0 ? true : false);
                await sessionStorage.SetItemAsync("darkmode", darkModeFound);
                darkmode = darkModeFound;

                var getWaitTimeSetting = (await client.GetFromJsonAsync<Settings>("api/settings/getwaittime"));
                int waitTimeInSecond = getWaitTimeSetting != null ? Int32.Parse(getWaitTimeSetting?.SettingValue!) : 0;
                DateTime date = DateTime.Now;
                var dateWithWaitTime = CurrencyExchange.CurrencyApiDate?.AddSeconds(waitTimeInSecond);
                if (CurrencyExchange.Rates == null && dateWithWaitTime <= date)
                {
                    try
                    {
                        CurrencyExchange.Rates = await CurrencyExchange.GetRatesAsync(client);
                    }
                    catch (Exception ex)
                    {
                        currency_api_error = $"Error - Type: {ex.GetType()}, Message: {ex.Message}";
                        if (ex.GetType().ToString() == "Microsoft.CSharp.RuntimeBinder.RuntimeBinderException")
                        {
                            currency_api_error = "currency_api_is_exceeded";
                        }
                    }

                    if (CurrencyExchange.Rates != null)
                    {
                        CurrencyExchange.CurrencyApiDate = new(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, 0, 0);
                    }
                }
            }
        }

        async Task LogoutClick()
        {
            PageHistoryState.ResetPageToHistory();
            await authStateProvider.Logout();
            navigationManager.NavigateTo("/login");
        }

        async Task darkMode()
        {
            if ((await AuthenticationState!).User.Identity!.IsAuthenticated)
            {
                if (darkmode)
                {
                    var settings = await client.GetFromJsonAsync<Settings[]>("api/settings/get");
                    var DarkModeSetting = (settings?.FirstOrDefault(x => x.SettingName == "darkmode" && x.SettingValue == (AuthenticationState!).Result.User.Identity?.Name));
                    await client.DeleteAsync($"api/settings/delete/{DarkModeSetting?.Id}");
                }
                else
                {
                    Settings setting = new();
                    setting.SettingValue = (AuthenticationState!).Result.User.Identity?.Name;
                    setting.SettingName = "darkmode";
                    await client.PostAsJsonAsync("api/settings/post", setting);
                }
            }
            else
            {
                await sessionStorage.SetItemAsync("darkmode", !await sessionStorage.GetItemAsync<bool>("darkmode"));
            }

            await OnParametersSetAsync();
        }
    }
}