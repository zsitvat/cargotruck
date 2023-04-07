using AutoMapper;
using Cargotruck.Server.Data;
using Cargotruck.Server.Models;
using Cargotruck.Server.Repositories;
using Cargotruck.Server.Repositories.Interfaces;
using Cargotruck.Server.Services;
using Cargotruck.Server.Services.Interfaces;
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

var configuration = new MapperConfiguration(cfg =>
{
    //Entity to  Dto
    cfg.CreateMap<Cargo, CargoDto>();
    cfg.CreateMap<Expense, ExpenseDto>();
    cfg.CreateMap<Login, LoginDto>();
    cfg.CreateMap<MonthlyExpense, MonthlyExpenseDto>();
    cfg.CreateMap<MonthlyExpense_task_expense, MonthlyExpense_task_expenseDto>();
    cfg.CreateMap<Privacies, PrivacyDto>();
    cfg.CreateMap<Road, RoadDto>();
    cfg.CreateMap<Setting, SettingDto>();
    cfg.CreateMap<DeliveryTask, DeliveryTaskDto>();
    cfg.CreateMap<Truck, TruckDto>();
    cfg.CreateMap<Warehouse, WarehouseDto>();

    //Dto to entity
    cfg.CreateMap<CargoDto, Cargo>().ForMember(entity => entity.Task, conf => conf.Ignore());
    cfg.CreateMap<ExpenseDto, Expense>();
    cfg.CreateMap<LoginDto, Login>();
    cfg.CreateMap<MonthlyExpenseDto, MonthlyExpense>().ForMember(entity => entity.Monthly_expenses_tasks_expenses, conf => conf.Ignore());
    cfg.CreateMap<MonthlyExpense_task_expenseDto, MonthlyExpense_task_expense>();
    cfg.CreateMap<PrivacyDto, Privacies>();
    cfg.CreateMap<RoadDto, Road>();
    cfg.CreateMap<SettingDto, Setting>();
    cfg.CreateMap<DeliveryTaskDto, DeliveryTask>().ForMember(entity => entity.Cargo, conf => conf.Ignore());
    cfg.CreateMap<TruckDto, Truck>();
    cfg.CreateMap<WarehouseDto, Warehouse>();
});
var mapper = configuration.CreateMapper();
builder.Services.AddSingleton(mapper);

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options => 
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//localization service
builder.Services.AddLocalization();

builder.Services.AddIdentity<User, IdentityRole>(options => 
    options.SignIn.RequireConfirmedPhoneNumber = false).AddEntityFrameworkStores<ApplicationDbContext>();

//Services
builder.Services.AddSingleton<IColumnNamesService, ColumnNamesService>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<IWarehouseService, WarehouseService>();
builder.Services.AddScoped<ITruckService, TruckService>();
builder.Services.AddScoped<ISettingService, SettingService>();
builder.Services.AddScoped<IPrivacyService,PrivacyService>();
builder.Services.AddScoped<IMonthlyExpenseService, MonthlyExpenseService>();
builder.Services.AddScoped<IExpenseService, ExpenseService>();
builder.Services.AddScoped<ICargoService, CargoService>();
builder.Services.AddScoped<IRoadService, RoadService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IErrorHandlerService, ErrorHandlerService>();

//Repositories
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<IWarehouseRepository, WarehouseRepository>();
builder.Services.AddScoped<ITruckRepository, TruckRepository>();
builder.Services.AddScoped<ISettingRepository, SettingRepository>();
builder.Services.AddScoped<IPrivacyRepository, PrivacyRepository>();
builder.Services.AddScoped<IMonthlyExpenseRepository, MonthlyExpenseRepository>();
builder.Services.AddScoped<IExpenseRepository, ExpenseRepository>();
builder.Services.AddScoped<ICargoRepository, CargoRepository>();
builder.Services.AddScoped<IRoadRepository, RoadRepository>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IAdminRepository, AdminRepository>();

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

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", builder =>
    {
        builder.WithOrigins("https://api.apilayer.com/exchangerates_data/")
               .AllowAnyHeader()
               .WithMethods("GET");
    });
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

app.UseCors("AllowSpecificOrigin");

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
