using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cargotruck.Server.Migrations
{
    public partial class _0323 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4b85ea10-1629-42cf-bff7-754cfab11b34");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "bc21cdf8-4617-44f7-87e5-b3670a8d35fa");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "d3a697cb-2188-4745-b463-41a9e5183ee9");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "f2b4c37a-aaae-41c7-b5de-28a2de89bb66");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "f70abf6d-fe85-4f8c-992c-e1914e078181");

            migrationBuilder.AlterColumn<long>(
                name: "Penalty",
                table: "Tasks",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "Payment",
                table: "Tasks",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "Final_Payment",
                table: "Tasks",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "Profit",
                table: "Monthly_Expenses",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "Expense",
                table: "Monthly_Expenses",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "Earning",
                table: "Monthly_Expenses",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "06090eee-8458-4c3d-bb0d-759b1757b200", "d8307b56-b117-4353-9c24-97d592e61237", "Accountant", "ACCOUNTANT" },
                    { "5d6b6125-2cef-4152-a416-1a40af7feecf", "ef52bbc8-0bb1-4ab3-8ebe-0fb7082c447d", "Storageman", "STORAGEMAN" },
                    { "65f85d12-0557-48c2-94e4-d96895cf1144", "08779b80-0a82-4be7-8138-57fd4bc0802c", "Admin", "ADMIN" },
                    { "e93d2d41-3e4f-4a7e-abaf-7ab87b5bc1eb", "51caaac6-be72-451d-a8f1-b51a232239bd", "Driver", "DRIVER" },
                    { "fd5059c3-b398-43b7-b30a-5a8190b69b19", "87ab9e44-60c4-4b57-bd0f-77effd753d55", "User", "USER" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "06090eee-8458-4c3d-bb0d-759b1757b200");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5d6b6125-2cef-4152-a416-1a40af7feecf");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "65f85d12-0557-48c2-94e4-d96895cf1144");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "e93d2d41-3e4f-4a7e-abaf-7ab87b5bc1eb");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "fd5059c3-b398-43b7-b30a-5a8190b69b19");

            migrationBuilder.AlterColumn<int>(
                name: "Penalty",
                table: "Tasks",
                type: "int",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Payment",
                table: "Tasks",
                type: "int",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Final_Payment",
                table: "Tasks",
                type: "int",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Profit",
                table: "Monthly_Expenses",
                type: "int",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Expense",
                table: "Monthly_Expenses",
                type: "int",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Earning",
                table: "Monthly_Expenses",
                type: "int",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "4b85ea10-1629-42cf-bff7-754cfab11b34", "bc76dd9f-0c30-4e30-b6fc-16f9cb00ceea", "User", "USER" },
                    { "bc21cdf8-4617-44f7-87e5-b3670a8d35fa", "5abcdeb8-7cdf-4db8-96d8-bbd3427e6b83", "Accountant", "ACCOUNTANT" },
                    { "d3a697cb-2188-4745-b463-41a9e5183ee9", "4008938c-d3c0-4cf0-b770-f956087a13bf", "Driver", "DRIVER" },
                    { "f2b4c37a-aaae-41c7-b5de-28a2de89bb66", "484ddac7-529b-433b-966e-b52fdc7a81d1", "Admin", "ADMIN" },
                    { "f70abf6d-fe85-4f8c-992c-e1914e078181", "4bc3a931-7a64-41d2-b914-051f59ff0527", "Storageman", "STORAGEMAN" }
                });
        }
    }
}
