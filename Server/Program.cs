using AutoMapper;
using Cargotruck.Client;
using Cargotruck.Server.Data;
using Cargotruck.Server.Models;
using Cargotruck.Server.Repositories;
using Cargotruck.Server.Services;
using Cargotruck.Shared.Model;
using Cargotruck.Shared.Model.Dto;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.StaticWebAssets;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Reflection;


/*
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
*/

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMvc().AddControllersAsServices();

builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());
var configuration = new MapperConfiguration(cfg =>
{
    cfg.CreateMap<Cargoes, CargoesDto>();
    cfg.CreateMap<Expenses, ExpensesDto>();
    cfg.CreateMap<Logins, LoginsDto>();
    cfg.CreateMap<Monthly_expenses, Monthly_expensesDto>();
    cfg.CreateMap<Privacies, PrivaciesDto>();
    cfg.CreateMap<Roads, RoadsDto>();
    cfg.CreateMap<Settings, SettingsDto>();
    cfg.CreateMap<Tasks, TasksDto>();
    cfg.CreateMap<Trucks, TrucksDto>();
    cfg.CreateMap<Warehouses, WarehousesDto>();

    cfg.CreateMap<CargoesDto, Cargoes>();
    cfg.CreateMap<ExpensesDto, Expenses>();
    cfg.CreateMap<LoginsDto, Logins>();
    cfg.CreateMap<Monthly_expensesDto, Monthly_expenses>();
    cfg.CreateMap<PrivaciesDto, Privacies>();
    cfg.CreateMap<RoadsDto, Roads>();
    cfg.CreateMap<SettingsDto, Settings>();
    cfg.CreateMap<TasksDto, Tasks>();
    cfg.CreateMap<TrucksDto, Trucks>();
    cfg.CreateMap<WarehousesDto, Warehouses>();
});
var mapper = configuration.CreateMapper();
builder.Services.AddSingleton(mapper);

//My services
builder.Services.AddSingleton<IColumnNamesService, ColumnNamesService>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<ITaskRepository, TaskRepository>();

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options => 
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//localization service
builder.Services.AddLocalization();

builder.Services.AddIdentity<Users, IdentityRole>(options => 
    options.SignIn.RequireConfirmedPhoneNumber = false).AddEntityFrameworkStores<ApplicationDbContext>();

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
    configuration.AssertConfigurationIsValid();
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
