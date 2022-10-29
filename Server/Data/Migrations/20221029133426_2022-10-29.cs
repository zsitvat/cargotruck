using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cargotruck.Server.Data.Migrations
{
    public partial class _20221029 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "23d5385e-efc8-46b3-b540-66826ad5382b");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "68c98b14-1437-4f63-9ef5-838e25410254");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "750b7fc3-6a43-4d2c-af37-a985fea54bf7");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "bda0d854-c8ae-4645-9d16-5196ac29cc68");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "dbba8bc8-f7a1-49cd-b9ba-ae7fcd78b194");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "3f546b6a-6175-4651-9b07-076025cd50db", "510f7ed2-6f64-4678-988a-179623626a52", "Storageman", "STORAGEMAN" },
                    { "6638f14b-ac44-4b25-a576-50f7bbabd57a", "03c2ea4a-2f3d-40ae-8768-222b89cfaeb0", "Manager", "MANAGER" },
                    { "6a1bf3ea-e84e-4961-9ce3-0e2fe2a54ab9", "5bf03066-525a-4eea-b742-32ff2b57bfb7", "Admin", "ADMIN" },
                    { "88c5ed45-d1e3-472b-a2d2-340ca2c3bb7e", "bffca9ae-f1d1-42ac-a6dd-db6a96a4c361", "Driver", "DRIVER" },
                    { "8ed7f2c1-4f25-4155-931c-1802c94ef8cb", "6c40f2f2-4d5a-4357-9d54-896810e7a238", "User", "USER" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3f546b6a-6175-4651-9b07-076025cd50db");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "6638f14b-ac44-4b25-a576-50f7bbabd57a");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "6a1bf3ea-e84e-4961-9ce3-0e2fe2a54ab9");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "88c5ed45-d1e3-472b-a2d2-340ca2c3bb7e");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "8ed7f2c1-4f25-4155-931c-1802c94ef8cb");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "23d5385e-efc8-46b3-b540-66826ad5382b", "c10c9289-32c5-451d-9c38-50fd547e2579", "Admin", "ADMIN" },
                    { "68c98b14-1437-4f63-9ef5-838e25410254", "46eb2260-5d33-4634-826a-462e4328a3a0", "Storageman", "STORAGEMAN" },
                    { "750b7fc3-6a43-4d2c-af37-a985fea54bf7", "6b2e27a4-7b67-467f-8da7-77ff803badd5", "User", "USER" },
                    { "bda0d854-c8ae-4645-9d16-5196ac29cc68", "bd5df26b-ff60-4559-9fa3-c17450e42593", "Driver", "DRIVER" },
                    { "dbba8bc8-f7a1-49cd-b9ba-ae7fcd78b194", "13fcfbc6-2524-471b-8c0c-2733792769b9", "Manager", "MANAGER" }
                });
        }
    }
}
