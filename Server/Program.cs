using Cargotruck.Data;
using Cargotruck.Server.Models;
using Cargotruck.Shared.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using System.Globalization;
using Microsoft.AspNetCore.Mvc.Localization;
using System.Reflection;
using System.IdentityModel.Tokens.Jwt;

// create appsettings.json if not exist
const string File_name = "appsettings.json";
if (!File.Exists(File_name) || new FileInfo(File_name).Length == 0)
{
    using (StreamWriter writer = new StreamWriter(File_name))
    {
        writer.Write("{\r\n  \"ConnectionStrings\": {\r\n    \"DefaultConnection\": \"Server=(localdb)\\\\mssqllocaldb;Database=CargoTruckDatabase");
        writer.Write(GetRandomString());
        writer.Write("; Trusted_Connection = True; MultipleActiveResultSets = true\",\r\n  },\r\n  \"Logging\": {\r\n    \"LogLevel\": {\r\n      \"Default\": \"Information\",\r\n      \"Microsoft\": \"Warning\",\r\n      \"Microsoft.Hosting.Lifetime\": \"Information\"\r\n    }\r\n  },\r\n  \"AllowedHosts\": \"*\"\r\n}");
    }
}

static string GetRandomString()
{
    var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
    var stringChars = new char[8];
    var random = new Random();
    for (int i = 0; i < stringChars.Length; i++)
    {
        stringChars[i] = chars[random.Next(chars.Length)];
    }

    var finalString = new String(stringChars);
    return finalString;
}


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddMvc().AddControllersAsServices();
// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
//localization service
builder.Services.AddLocalization();


CultureInfo[] supportedCultures = new[]
{
    new CultureInfo("hu"),
    new CultureInfo("en-US")
};

builder.Services.Configure<RequestLocalizationOptions>(options =>
    {
        options.SetDefaultCulture("hu");
        options.DefaultRequestCulture = new RequestCulture("hu");
        options.SupportedCultures = supportedCultures;
        options.SupportedUICultures = supportedCultures;
        options.RequestCultureProviders = new List<IRequestCultureProvider>
        {
            new QueryStringRequestCultureProvider(),
            new CookieRequestCultureProvider()
        };
        options.FallBackToParentUICultures = true;
        options.RequestCultureProviders.Clear();
    });
CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("hu");
CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("hu");

builder.Services.AddIdentity<Users, IdentityRole>(options => options.SignIn.RequireConfirmedPhoneNumber = false).AddEntityFrameworkStores<ApplicationDbContext>();


builder.Services.ConfigureApplicationCookie(options =>
{
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = false;
    options.Events.OnRedirectToLogin = context =>
    {  
        context.Response.StatusCode = 401;
        return Task.CompletedTask;
    };
});
builder.Services.AddControllers().AddNewtonsoftJson();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();




var app = builder.Build();

// add the appDbContext service
using (var scope = app.Services.CreateAsyncScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseRequestLocalization(new RequestLocalizationOptions
{
    ApplyCurrentCultureToResponseHeaders = true
});
app.UseHttpsRedirection();
app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();


app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
