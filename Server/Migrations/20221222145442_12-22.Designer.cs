﻿// <auto-generated />
using System;
using Cargotruck.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Cargotruck.Server.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20221222145442_12-22")]
    partial class _1222
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("Cargotruck.Server.Models.Users", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers", (string)null);
                });

            modelBuilder.Entity("Cargotruck.Shared.Models.Cargoes", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int?>("Cost_of_storage")
                        .HasColumnType("int");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<string>("Delivery_requirements")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("Storage_starting_time")
                        .HasColumnType("datetime2");

                    b.Property<int>("Task_id")
                        .HasColumnType("int");

                    b.Property<int>("User_id")
                        .HasColumnType("int");

                    b.Property<string>("Vehicle_registration_number")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("Warehouse_id")
                        .HasColumnType("int");

                    b.Property<string>("Warehouse_section")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("WarehousesId")
                        .HasColumnType("int");

                    b.Property<string>("Weight")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("WarehousesId");

                    b.ToTable("Cargoes");
                });

            modelBuilder.Entity("Cargotruck.Shared.Models.Expenses", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"), 1L, 1);

                    b.Property<int>("Cost_of_storage")
                        .HasColumnType("int");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<int>("Driver_salary")
                        .HasColumnType("int");

                    b.Property<int>("Driver_spending")
                        .HasColumnType("int");

                    b.Property<int>("Fuel")
                        .HasColumnType("int");

                    b.Property<int?>("Monthly_expensesId")
                        .HasColumnType("int");

                    b.Property<int>("Other")
                        .HasColumnType("int");

                    b.Property<int>("Penalty")
                        .HasColumnType("int");

                    b.Property<int>("Repair_cost")
                        .HasColumnType("int");

                    b.Property<string>("Repair_description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Road_fees")
                        .HasColumnType("int");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Type_id")
                        .HasColumnType("int");

                    b.Property<long>("User_id")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("Monthly_expensesId");

                    b.ToTable("Expenses");
                });

            modelBuilder.Entity("Cargotruck.Shared.Models.Monthly_expenses", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<int>("Earning")
                        .HasColumnType("int");

                    b.Property<int>("Profit")
                        .HasColumnType("int");

                    b.Property<int>("User_id")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Monthly_Expenses");
                });

            modelBuilder.Entity("Cargotruck.Shared.Models.Roads", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<string>("Direction")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("Ending_date")
                        .HasColumnType("datetime2");

                    b.Property<string>("Ending_place")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("Expenses_id")
                        .HasColumnType("int");

                    b.Property<int?>("Id_cargo")
                        .HasColumnType("int");

                    b.Property<string>("Purpose_of_the_trip")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("Starting_date")
                        .IsRequired()
                        .HasColumnType("datetime2");

                    b.Property<string>("Starting_place")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("Task_id")
                        .HasColumnType("int");

                    b.Property<int>("User_id")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Roads");
                });

            modelBuilder.Entity("Cargotruck.Shared.Models.Tasks", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<bool>("Completed")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("Completion_time")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("Final_Payment")
                        .HasColumnType("int");

                    b.Property<string>("Id_cargo")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("Monthly_expensesId")
                        .HasColumnType("int");

                    b.Property<string>("Other_stops")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Partner")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("Payment")
                        .HasColumnType("int");

                    b.Property<int?>("Penalty")
                        .HasColumnType("int");

                    b.Property<string>("Place_of_delivery")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Place_of_receipt")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Storage_time")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Time_of_delay")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("Time_of_delivery")
                        .IsRequired()
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("Time_of_receipt")
                        .IsRequired()
                        .HasColumnType("datetime2");

                    b.Property<long>("User_id")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("Monthly_expensesId");

                    b.ToTable("Tasks");
                });

            modelBuilder.Entity("Cargotruck.Shared.Models.Trucks", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Brand")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<string>("Max_weight")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Road_id")
                        .HasColumnType("int");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("User_id")
                        .HasColumnType("int");

                    b.Property<int>("Vehicle_registration_number")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Trucks");
                });

            modelBuilder.Entity("Cargotruck.Shared.Models.Warehouses", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<string>("Owner")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("User_id")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Warehouses");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles", (string)null);

                    b.HasData(
                        new
                        {
                            Id = "f477b862-6f2b-4baa-bf6c-556e7c311d65",
                            ConcurrencyStamp = "f95d0a85-7258-4627-97a5-96502bfea660",
                            Name = "User",
                            NormalizedName = "USER"
                        },
                        new
                        {
                            Id = "4a53bfa1-27ab-469e-82d6-d479bd4b1b31",
                            ConcurrencyStamp = "571a5a99-3f13-439d-9122-82fc540b9679",
                            Name = "Admin",
                            NormalizedName = "ADMIN"
                        },
                        new
                        {
                            Id = "526a29c1-463e-48a4-86b2-b46806f25237",
                            ConcurrencyStamp = "3089774a-ddca-43be-a20e-6f7cd55c6a49",
                            Name = "Driver",
                            NormalizedName = "DRIVER"
                        },
                        new
                        {
                            Id = "f14cafe0-b60f-4498-a6ba-ec677d97d771",
                            ConcurrencyStamp = "db941db3-aa63-4b4c-b505-303b46100421",
                            Name = "Manager",
                            NormalizedName = "MANAGER"
                        },
                        new
                        {
                            Id = "f336719c-8840-4328-8e7d-12b8cdf0025a",
                            ConcurrencyStamp = "5e66b045-0515-4f92-936a-8bbd594c4574",
                            Name = "Storageman",
                            NormalizedName = "STORAGEMAN"
                        });
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("RoleId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens", (string)null);
                });

            modelBuilder.Entity("Cargotruck.Shared.Models.Cargoes", b =>
                {
                    b.HasOne("Cargotruck.Shared.Models.Warehouses", null)
                        .WithMany("Cargo_id")
                        .HasForeignKey("WarehousesId");
                });

            modelBuilder.Entity("Cargotruck.Shared.Models.Expenses", b =>
                {
                    b.HasOne("Cargotruck.Shared.Models.Monthly_expenses", null)
                        .WithMany("Expenses")
                        .HasForeignKey("Monthly_expensesId");
                });

            modelBuilder.Entity("Cargotruck.Shared.Models.Tasks", b =>
                {
                    b.HasOne("Cargotruck.Shared.Models.Monthly_expenses", null)
                        .WithMany("Task_id")
                        .HasForeignKey("Monthly_expensesId");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("Cargotruck.Server.Models.Users", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("Cargotruck.Server.Models.Users", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Cargotruck.Server.Models.Users", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("Cargotruck.Server.Models.Users", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Cargotruck.Shared.Models.Monthly_expenses", b =>
                {
                    b.Navigation("Expenses");

                    b.Navigation("Task_id");
                });

            modelBuilder.Entity("Cargotruck.Shared.Models.Warehouses", b =>
                {
                    b.Navigation("Cargo_id");
                });
#pragma warning restore 612, 618
        }
    }
}