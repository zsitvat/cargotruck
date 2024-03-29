using Blazored.SessionStorage;
using Cargotruck.Client;
using Cargotruck.Client.Services;
using Cargotruck.Client.Services.Interfaces;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;
using System.Globalization;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddLocalization();

builder.Services.AddOptions();

builder.Services.AddAuthorizationCore();

builder.Services.AddScoped<CustomStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(s => s.GetRequiredService<CustomStateProvider>());
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped <PageHistoryState>(); //back to prievouse page
builder.Services.AddScoped<IFileDownload, FileDownload>();
builder.Services.AddSingleton<ICurrencyExchange, CurrencyExchange>();

builder.Services.AddBlazoredSessionStorage();

var host = builder.Build();

CultureInfo culture;
var js = host.Services.GetRequiredService<IJSRuntime>();

var result = await js.InvokeAsync<string>("blazorCulture.get");
if (result != null)
{
    culture = new CultureInfo(result);
}
else
{
    culture = new CultureInfo("hu");
    await js.InvokeVoidAsync("blazorCulture.set", "hu");
}

CultureInfo.DefaultThreadCurrentCulture = culture;
CultureInfo.DefaultThreadCurrentUICulture = culture;

await host.RunAsync();