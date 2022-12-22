using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cargotruck.Server.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "2cac01b0-13a3-4866-901e-7bd21ac2f44f", "be7d4311-1194-47c3-b97a-90aa8e5fe228", "Driver", "DRIVER" },
                    { "472cc336-cfab-462c-a642-c0d86019b3c7", "fc610824-2d5c-4eca-8bb9-fe0eb928ec14", "Manager", "MANAGER" },
                    { "72e25072-ad65-4311-8bb7-3d5ae76a51b0", "40b1346f-2cf9-486a-9b15-794d2a5728a5", "User", "USER" },
                    { "b50ef9ba-8574-45d0-8b13-a040052d233f", "2868cee8-1b66-4553-9126-57b2207ec69c", "Admin", "ADMIN" },
                    { "d881e315-7fa9-43f4-9a3b-ff1ab1a86d6b", "a6d79333-bfa2-439d-9c66-730e6719675c", "Storageman", "STORAGEMAN" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2cac01b0-13a3-4866-901e-7bd21ac2f44f");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "472cc336-cfab-462c-a642-c0d86019b3c7");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "72e25072-ad65-4311-8bb7-3d5ae76a51b0");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b50ef9ba-8574-45d0-8b13-a040052d233f");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "d881e315-7fa9-43f4-9a3b-ff1ab1a86d6b");

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
    }
}
