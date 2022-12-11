using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cargotruck.Server.Migrations
{
    public partial class _221211 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0805c705-14ca-4cd3-a529-8000d7603bbd");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "208e7617-5fb3-4a89-8720-b5e63dbec334");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b3c5b849-3e42-4a84-ae68-2d19a236dd97");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "caf5cb16-c862-481e-bffa-a6fed7ee8a28");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "d88db4a3-d095-4c8b-a1e0-58ce9fc2ca3f");

            migrationBuilder.AlterColumn<string>(
                name: "Time_of_delay",
                table: "Tasks",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Storage_time",
                table: "Tasks",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "Penalty",
                table: "Tasks",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "Payment",
                table: "Tasks",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Other_stops",
                table: "Tasks",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Id_cargo",
                table: "Tasks",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "Final_Payment",
                table: "Tasks",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Tasks",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

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

        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AlterColumn<string>(
                name: "Time_of_delay",
                table: "Tasks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Storage_time",
                table: "Tasks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Penalty",
                table: "Tasks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Payment",
                table: "Tasks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Other_stops",
                table: "Tasks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Id_cargo",
                table: "Tasks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Final_Payment",
                table: "Tasks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Tasks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "0805c705-14ca-4cd3-a529-8000d7603bbd", "4d286890-4556-4889-a5f8-06b1b3f6cd31", "Driver", "DRIVER" },
                    { "208e7617-5fb3-4a89-8720-b5e63dbec334", "dc0c9ab1-2c68-4fa4-bd3a-cc8b53dbf293", "Storageman", "STORAGEMAN" },
                    { "b3c5b849-3e42-4a84-ae68-2d19a236dd97", "1ce44a17-e597-475c-adbf-24bbabc1cfbc", "User", "USER" },
                    { "caf5cb16-c862-481e-bffa-a6fed7ee8a28", "c123bbc4-3245-44a3-a25e-279216963f84", "Admin", "ADMIN" },
                    { "d88db4a3-d095-4c8b-a1e0-58ce9fc2ca3f", "d3e19259-c334-4107-91e2-8178dd36da67", "Manager", "MANAGER" }
                });
        }
    }
}
