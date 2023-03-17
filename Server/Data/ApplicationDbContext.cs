using Cargotruck.Server.Models;
using Cargotruck.Shared.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Cargotruck.Server.Data
{
    public class ApplicationDbContext : IdentityDbContext<Users>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            //create the database for mainly testing in local
            //Database.EnsureCreated();
            Database.Migrate();
        }
        public override DbSet<Users> Users { get; set; } = default!;
        public DbSet<Tasks> Tasks { get; set; } = default!;
        public DbSet<Cargoes> Cargoes { get; set; } = default!;
        public DbSet<Expenses> Expenses { get; set; } = default!;
        public DbSet<Monthly_expenses> Monthly_Expenses { get; set; } = default!;
        public DbSet<Monthly_expenses_tasks_expenses> Monthly_expenses_tasks_expenses { get; set; } = default!;
        public DbSet<Roads> Roads { get; set; } = default!;
        public DbSet<Trucks> Trucks { get; set; } = default!;
        public DbSet<Warehouses> Warehouses { get; set; } = default!;
        public DbSet<Privacies> Privacies { get; set; } = default!;
        public DbSet<Logins> Logins { get; set; } = default!;
        public DbSet<Settings> Settings { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfiguration(new RoleConfiguration());
        }
    }
}