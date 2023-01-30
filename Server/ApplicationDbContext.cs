using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Cargotruck.Shared.Models;
using Cargotruck.Server.Data;
using System.Security.Cryptography.X509Certificates;
using Cargotruck.Server.Models;
using System.Reflection.Metadata;

namespace Cargotruck.Data
{
    public class ApplicationDbContext : IdentityDbContext<Users>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            //create the database for mainly testing in local
            Database.EnsureCreated();
            //Database.Migrate();
        }
        public DbSet<Users> Users { get; set; }
        public DbSet<Tasks> Tasks { get; set; }
        public DbSet<Cargoes> Cargoes { get; set; }
        public DbSet<Expenses> Expenses { get; set; }
        public DbSet<Monthly_expenses> Monthly_Expenses { get; set; }
        public DbSet<Monthly_expenses_tasks_expenses> Monthly_expenses_tasks_expenses { get; set; }
        public DbSet<Roads> Roads { get; set; }
        public DbSet<Trucks> Trucks { get; set; }
        public DbSet<Warehouses> Warehouses { get; set; }
        public DbSet<Privacies> Privacies { get; set; }
        public DbSet<Logins> Logins { get; set; }
        public DbSet<Settings> Settings { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfiguration(new RoleConfiguration());
        }
    }
}