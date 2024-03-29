﻿using Cargotruck.Server.Models;
using Cargotruck.Shared.Model;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Metrics;

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
        public DbSet<DeliveryTask> Tasks { get; set; } = default!;
        public DbSet<Cargo> Cargoes { get; set; } = default!;
        public DbSet<Expense> Expenses { get; set; } = default!;
        public DbSet<MonthlyExpense> MonthlyExpenses { get; set; } = default!;
        public DbSet<MonthlyExpense_task_expense> MonthlyExpensesTasksExpenses { get; set; } = default!;
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

            builder.Entity<DeliveryTask>()
            .HasOne(a => a.Cargo)
            .WithOne(a => a.Task)
            .HasForeignKey<Cargo>(c => c.TaskId);

        }
    }
}