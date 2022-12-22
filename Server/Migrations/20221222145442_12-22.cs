using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cargotruck.Server.Migrations
{
    public partial class _1222 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2d213b5b-cfbf-4d09-9ad7-dbc90da4f6d2");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4d7a2ece-325f-42bd-b022-c98be1185037");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c185ebb1-b5a4-403b-9054-bde5a35f3873");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "e4d39407-9c32-42dc-9227-99d6b5544c77");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "f16f2c6d-600b-4bd6-a0c4-ef62e9cf3dc5");

            migrationBuilder.AlterColumn<int>(
                name: "Task_id",
                table: "Roads",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "Id_cargo",
                table: "Roads",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "Expenses_id",
                table: "Roads",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Ending_date",
                table: "Roads",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "Warehouse_section",
                table: "Cargoes",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "Warehouse_id",
                table: "Cargoes",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Vehicle_registration_number",
                table: "Cargoes",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Storage_starting_time",
                table: "Cargoes",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Cargoes",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Delivery_requirements",
                table: "Cargoes",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "Cost_of_storage",
                table: "Cargoes",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "4a53bfa1-27ab-469e-82d6-d479bd4b1b31", "571a5a99-3f13-439d-9122-82fc540b9679", "Admin", "ADMIN" },
                    { "526a29c1-463e-48a4-86b2-b46806f25237", "3089774a-ddca-43be-a20e-6f7cd55c6a49", "Driver", "DRIVER" },
                    { "f14cafe0-b60f-4498-a6ba-ec677d97d771", "db941db3-aa63-4b4c-b505-303b46100421", "Manager", "MANAGER" },
                    { "f336719c-8840-4328-8e7d-12b8cdf0025a", "5e66b045-0515-4f92-936a-8bbd594c4574", "Storageman", "STORAGEMAN" },
                    { "f477b862-6f2b-4baa-bf6c-556e7c311d65", "f95d0a85-7258-4627-97a5-96502bfea660", "User", "USER" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4a53bfa1-27ab-469e-82d6-d479bd4b1b31");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "526a29c1-463e-48a4-86b2-b46806f25237");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "f14cafe0-b60f-4498-a6ba-ec677d97d771");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "f336719c-8840-4328-8e7d-12b8cdf0025a");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "f477b862-6f2b-4baa-bf6c-556e7c311d65");

            migrationBuilder.AlterColumn<int>(
                name: "Task_id",
                table: "Roads",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Id_cargo",
                table: "Roads",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Expenses_id",
                table: "Roads",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "Ending_date",
                table: "Roads",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Warehouse_section",
                table: "Cargoes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Warehouse_id",
                table: "Cargoes",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Vehicle_registration_number",
                table: "Cargoes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "Storage_starting_time",
                table: "Cargoes",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Cargoes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Delivery_requirements",
                table: "Cargoes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Cost_of_storage",
                table: "Cargoes",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "2d213b5b-cfbf-4d09-9ad7-dbc90da4f6d2", "b6b1b03d-8c07-4a3d-9089-0b1accaf020d", "Admin", "ADMIN" },
                    { "4d7a2ece-325f-42bd-b022-c98be1185037", "267ab11c-7c6f-4e5d-b102-9ee16a3a10af", "Manager", "MANAGER" },
                    { "c185ebb1-b5a4-403b-9054-bde5a35f3873", "6959df92-b49f-4f27-9979-004511d6d641", "Driver", "DRIVER" },
                    { "e4d39407-9c32-42dc-9227-99d6b5544c77", "efbff01f-e770-4c9d-8283-3549f2fbe637", "User", "USER" },
                    { "f16f2c6d-600b-4bd6-a0c4-ef62e9cf3dc5", "bb4863a1-f81d-4f3f-b2ae-f415364cc6c1", "Storageman", "STORAGEMAN" }
                });
        }
    }
}
