using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Cargotruck.Shared.Models;
using Cargotruck.Server.Models;

namespace Cargotruck.Data
{
    public class ApplicationDbContext : IdentityDbContext<Users>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            //create the database for mainly testing in local
            Database.EnsureCreated();
            Database.Migrate();
        } 
        
        public DbSet<Tasks> Tasks { get; set; }
        public DbSet<Cargoes> Cargoes { get; set; }
        public DbSet<Expenses> Expenses { get; set; }
        public DbSet<Monthly_expenses> Monthly_Expenses { get; set; }
        public DbSet<Roads> Roads { get; set; }
        public DbSet<Trucks> Trucks { get; set; }
        public DbSet<Warehouses> Warehouses { get; set; }
    }
}