using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Cargotruck.Server.Model;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Cargotruck.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            //create the database for mainly testing in local
            //Database.EnsureCreated();
            //Database.Migrate();
        }
        public DbSet<Users>? Users { get; set; }
        public DbSet<Tasks>? Tasks { get; set; }
        public DbSet<Cargoes>? Cargoes { get; set; }
       /* public DbSet<Expenses>? Expenses { get; set; }
        public DbSet<Monthly_expenses>? Monthly_Expenses { get; set; }
        public DbSet<Roads>? Roads { get; set; }
        public DbSet<Trucks>? Trucks { get; set; }
        public DbSet<Warehouses>? Warehouses { get; set; }*/
    }
}