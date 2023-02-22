using Cargotruck.Data;
using Cargotruck.Server.Models;
using Microsoft.AspNetCore.Hosting.StaticWebAssets;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

// create appsettings.json if not exist
const string File_name = "appsettings.json";
if (!File.Exists(File_name) || new FileInfo(File_name).Length == 0)
{
    using StreamWriter writer = new(File_name);
    writer.Write("{\r\n  \"ConnectionStrings\": {\r\n    \"DefaultConnection\": \"Server=(localdb)\\\\mssqllocaldb;Database=CargoTruckDatabase");
    writer.Write(GetRandomString());
    writer.Write("; Trusted_Connection = True; MultipleActiveResultSets = true\",\r\n  },\r\n  \"Logging\": {\r\n    \"LogLevel\": {\r\n      \"Default\": \"Information\",\r\n      \"Microsoft\": \"Warning\",\r\n      \"Microsoft.Hosting.Lifetime\": \"Information\"\r\n    }\r\n  },\r\n  \"AllowedHosts\": \"*\"\r\n}");
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

builder.Services.AddIdentity<Users, IdentityRole>(options => options.SignIn.RequireConfirmedPhoneNumber = false).AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = false;
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
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
    app.UseHsts();
}


StaticWebAssetsLoader.UseStaticWebAssets(builder.Environment, builder.Configuration);
app.UseCookiePolicy(new CookiePolicyOptions
{
    MinimumSameSitePolicy = SameSiteMode.None
});

app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("hu"),
    SupportedCultures = new[] { new CultureInfo("en-US"), new CultureInfo("hu") },
    SupportedUICultures = new[] { new CultureInfo("en-US"), new CultureInfo("hu") },
    RequestCultureProviders = new[]
    {
        new QueryStringRequestCultureProvider(),
    }
});

app.UseAuthentication();
app.UseHttpsRedirection();
app.UseBlazorFrameworkFiles();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
