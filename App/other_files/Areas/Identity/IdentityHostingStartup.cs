using System;
using App.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AppContext = App.Data.AppContext;

[assembly: HostingStartup(typeof(App.Areas.Identity.IdentityHostingStartup))]
namespace App.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddDbContext<AppContext>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString("AppContextConnection")));

                
            });
        }
    }
}