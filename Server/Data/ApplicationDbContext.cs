using Cargotruck.Server.Models;
using Cargotruck.Shared.Model;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Cargotruck.Server.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            //create the database for mainly testing in local
            //Database.EnsureCreated();
            Database.Migrate();
        }
        public override DbSet<User> Users { get; set; } = default!;
        public DbSet<Shared.Model.Task> Tasks { get; set; } = default!;
        public DbSet<Cargo> Cargoes { get; set; } = default!;
        public DbSet<Expense> Expenses { get; set; } = default!;
        public DbSet<Monthly_expense> Monthly_Expenses { get; set; } = default!;
        public DbSet<Monthly_expense_task_expense> Monthly_expenses_tasks_expenses { get; set; } = default!;
        public DbSet<Road> Roads { get; set; } = default!;
        public DbSet<Truck> Trucks { get; set; } = default!;
        public DbSet<Warehouse> Warehouses { get; set; } = default!;
        public DbSet<Privacies> Privacies { get; set; } = default!;
        public DbSet<Login> Logins { get; set; } = default!;
        public DbSet<Setting> Settings { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfiguration(new RoleConfiguration());
        }
    }
}