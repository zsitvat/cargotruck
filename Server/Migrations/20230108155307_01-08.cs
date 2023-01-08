using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cargotruck.Server.Migrations
{
    public partial class _0108 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cargoes_Warehouses_WarehousesId",
                table: "Cargoes");

            migrationBuilder.DropForeignKey(
                name: "FK_Expenses_Monthly_Expenses_Monthly_expensesId",
                table: "Expenses");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Monthly_Expenses_Monthly_expensesId",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_Monthly_expensesId",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Expenses_Monthly_expensesId",
                table: "Expenses");

            migrationBuilder.DropIndex(
                name: "IX_Cargoes_WarehousesId",
                table: "Cargoes");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0e44b775-ac1f-4689-a444-760161780ceb");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "21ab7070-eedf-48e2-8532-68bb5f93c20d");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "51120a95-a118-41d4-901c-97a035be568f");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "8a917ccb-8ba8-400a-a816-0d49ef1900cb");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b07245d8-8a3d-41ab-b589-c59516dd45b9");

            migrationBuilder.DropColumn(
                name: "Monthly_expensesId",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "Monthly_expensesId",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "WarehousesId",
                table: "Cargoes");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Monthly_Expenses",
                newName: "Monthly_expense_id");

            migrationBuilder.AlterColumn<string>(
                name: "User_id",
                table: "Warehouses",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "User_id",
                table: "Trucks",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "User_id",
                table: "Tasks",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<int>(
                name: "Id_cargo",
                table: "Tasks",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "User_id",
                table: "Roads",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Ending_place",
                table: "Roads",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "User_id",
                table: "Monthly_Expenses",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "Profit",
                table: "Monthly_Expenses",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "Earning",
                table: "Monthly_Expenses",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "Expense",
                table: "Monthly_Expenses",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "User_id",
                table: "Expenses",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<int>(
                name: "Type_id",
                table: "Expenses",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "User_id",
                table: "Cargoes",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateTable(
                name: "Monthly_expenses_tasks_expenses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Monthly_expense_id = table.Column<int>(type: "int", nullable: false),
                    Expense_id = table.Column<int>(type: "int", nullable: true),
                    Task_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Monthly_expenses_tasks_expenses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Monthly_expenses_tasks_expenses_Monthly_Expenses_Monthly_expense_id",
                        column: x => x.Monthly_expense_id,
                        principalTable: "Monthly_Expenses",
                        principalColumn: "Monthly_expense_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Privacy",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    text_eng = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    text_hun = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Privacy", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "523b7160-c32e-4302-b85a-69a127a13589", "f06f9752-2dd5-4130-94ab-7cdec195543d", "Storageman", "STORAGEMAN" },
                    { "999fb4a4-de3e-40cb-8485-f3b75fdd83d3", "17d91327-a003-447c-b7b7-7d78b251da2e", "Driver", "DRIVER" },
                    { "b396ea11-fbfb-48d2-98a5-702bdea7f14d", "8b6a2bca-e5dd-4966-8127-10b5b10bae1f", "User", "USER" },
                    { "bd116c37-74eb-4e70-88c2-a92dc4faf3b3", "4238a4fb-7067-4b4b-924f-c672fa8cc7b3", "Manager", "MANAGER" },
                    { "ec392133-b5a2-41b5-ae8f-311b9ee0d273", "cb8d8577-3043-4ff9-829d-0d8162dceb09", "Admin", "ADMIN" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Monthly_expenses_tasks_expenses_Monthly_expense_id",
                table: "Monthly_expenses_tasks_expenses",
                column: "Monthly_expense_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Monthly_expenses_tasks_expenses");

            migrationBuilder.DropTable(
                name: "Privacy");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "523b7160-c32e-4302-b85a-69a127a13589");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "999fb4a4-de3e-40cb-8485-f3b75fdd83d3");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b396ea11-fbfb-48d2-98a5-702bdea7f14d");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "bd116c37-74eb-4e70-88c2-a92dc4faf3b3");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ec392133-b5a2-41b5-ae8f-311b9ee0d273");

            migrationBuilder.DropColumn(
                name: "Expense",
                table: "Monthly_Expenses");

            migrationBuilder.RenameColumn(
                name: "Monthly_expense_id",
                table: "Monthly_Expenses",
                newName: "Id");

            migrationBuilder.AlterColumn<int>(
                name: "User_id",
                table: "Warehouses",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "User_id",
                table: "Trucks",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "User_id",
                table: "Tasks",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Id_cargo",
                table: "Tasks",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Monthly_expensesId",
                table: "Tasks",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "User_id",
                table: "Roads",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Ending_place",
                table: "Roads",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "User_id",
                table: "Monthly_Expenses",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Profit",
                table: "Monthly_Expenses",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Earning",
                table: "Monthly_Expenses",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "User_id",
                table: "Expenses",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Type_id",
                table: "Expenses",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Monthly_expensesId",
                table: "Expenses",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "User_id",
                table: "Cargoes",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WarehousesId",
                table: "Cargoes",
                type: "int",
                nullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "0e44b775-ac1f-4689-a444-760161780ceb", "1ad788f3-e77d-40b5-814a-88d12d5f563f", "Manager", "MANAGER" },
                    { "21ab7070-eedf-48e2-8532-68bb5f93c20d", "218b7161-88dc-4b59-8fe0-e0275f933778", "Storageman", "STORAGEMAN" },
                    { "51120a95-a118-41d4-901c-97a035be568f", "60cfedcc-914c-4fa2-add0-50ec7c962a09", "Admin", "ADMIN" },
                    { "8a917ccb-8ba8-400a-a816-0d49ef1900cb", "26aa68da-80f7-48c8-9b6f-69ee15dfa2d5", "User", "USER" },
                    { "b07245d8-8a3d-41ab-b589-c59516dd45b9", "9c211020-5447-4af0-a311-d95bdd3fcc5e", "Driver", "DRIVER" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_Monthly_expensesId",
                table: "Tasks",
                column: "Monthly_expensesId");

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_Monthly_expensesId",
                table: "Expenses",
                column: "Monthly_expensesId");

            migrationBuilder.CreateIndex(
                name: "IX_Cargoes_WarehousesId",
                table: "Cargoes",
                column: "WarehousesId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cargoes_Warehouses_WarehousesId",
                table: "Cargoes",
                column: "WarehousesId",
                principalTable: "Warehouses",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Expenses_Monthly_Expenses_Monthly_expensesId",
                table: "Expenses",
                column: "Monthly_expensesId",
                principalTable: "Monthly_Expenses",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Monthly_Expenses_Monthly_expensesId",
                table: "Tasks",
                column: "Monthly_expensesId",
                principalTable: "Monthly_Expenses",
                principalColumn: "Id");
        }
    }
}
