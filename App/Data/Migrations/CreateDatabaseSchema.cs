using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace App.Data.Migrations
{
    public partial class CreateDatabaseSchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    UserName = table.Column<string>(maxLength: 256, nullable: true),
                    Email = table.Column<string>(maxLength: 256, nullable: true),
                    EmailComfirmed = table.Column<bool>(maxLength: 1, nullable: false, defaultValue:false),
                    PasswordHash = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "Users",
                column: "Email");
/*
            migrationBuilder.CreateTable( 
                name: "Tasks",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    Partner = table.Column<string>(maxLength: 256, nullable: true),
                    Description = table.Column<string>(maxLength: 1024, nullable: true),
                    Place_of_receipt = table.Column<string>(maxLength: 512, nullable: true, defaultValue:' '),
                    Time_of_receipt = table.Column<DateTime>(maxLength: 20, nullable: false),
                    Place_of_delivery = table.Column<string>(maxLength: 512, nullable: true, defaultValue:' '),
                    Time_of_delivery = table.Column<DateTime>(maxLength: 20, nullable: false),
                    Other_stops = table.Column<string>(maxLength: 256, nullable: true),
                    Id_cargo = table.Column<string>(nullable: false),
                    Storage_time = table.Column<string>(maxLength: 256, nullable: true),
                    Completed = table.Column<bool>(maxLength: 1, nullable: false, defaultValue: false),
                    Completion_time = table.Column<DateTime>(maxLength: 256, nullable: true),
                    Time_of_delay = table.Column<string>(maxLength: 256, nullable: true),
                    Payment = table.Column<string>(maxLength: 256, nullable: true),
                    Final_Payment = table.Column<string>(maxLength: 256, nullable: true),
                    Penalty = table.Column<string>(maxLength: 256, nullable: true),
                    Date_of_creation = table.Column<DateTime>(nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tasks", x => x.Id);
                });
*/
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users");
            migrationBuilder.DropTable(
                name: "Tasks");
        }
    }
}
