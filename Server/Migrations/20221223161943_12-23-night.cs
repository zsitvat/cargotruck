using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cargotruck.Server.Migrations
{
    public partial class _1223night : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2193d7fe-8f22-4d1b-b5e5-21b6e200aa97");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "427f7ff0-d42a-45fc-89bd-f95379ae6daa");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "718a7e27-d0b2-412b-959c-ad3c45eb23ee");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "bac832b1-6745-4d0a-a69f-3d0b989dd34e");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "f4bf842c-8164-4026-8c68-85b364e6cb07");

            migrationBuilder.DropColumn(
                name: "Cost_of_storage",
                table: "Cargoes");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Expenses",
                type: "int",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .Annotation("SqlServer:Identity", "1, 1")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "0f680424-3a6b-4af3-83ea-c51c9ce7cfd1", "fe9950f4-f2d0-4179-994e-58c8b6661a99", "Storageman", "STORAGEMAN" },
                    { "55c6009e-8864-42a3-b1de-744eed65311a", "1280ff5b-cae3-425a-9e8d-285d3c8d0b57", "Admin", "ADMIN" },
                    { "68761aea-f564-4208-bb52-fa05a9108639", "d3e78156-115c-4182-a814-30fce75fe4ea", "Manager", "MANAGER" },
                    { "d8b117a2-c5e0-414d-87db-3ca1e03dde0f", "11de5b65-c2bc-4d8c-997f-c1b5b5fd65d2", "Driver", "DRIVER" },
                    { "dd4727a2-cd39-4fd1-8e6a-e9f25fbafa87", "805cef27-f602-4528-b775-50afe82aa918", "User", "USER" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0f680424-3a6b-4af3-83ea-c51c9ce7cfd1");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "55c6009e-8864-42a3-b1de-744eed65311a");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "68761aea-f564-4208-bb52-fa05a9108639");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "d8b117a2-c5e0-414d-87db-3ca1e03dde0f");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "dd4727a2-cd39-4fd1-8e6a-e9f25fbafa87");

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Expenses",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("SqlServer:Identity", "1, 1")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "Cost_of_storage",
                table: "Cargoes",
                type: "int",
                nullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "2193d7fe-8f22-4d1b-b5e5-21b6e200aa97", "2c0a39f1-dc22-4080-a316-a9228685387d", "User", "USER" },
                    { "427f7ff0-d42a-45fc-89bd-f95379ae6daa", "14efb58c-2325-46ce-989b-ab3989b848d0", "Admin", "ADMIN" },
                    { "718a7e27-d0b2-412b-959c-ad3c45eb23ee", "524de49c-88b8-4614-9c21-84ad31c9c5b8", "Driver", "DRIVER" },
                    { "bac832b1-6745-4d0a-a69f-3d0b989dd34e", "1bc3a6d2-6410-4aec-81e7-9a7bf4d42a5a", "Manager", "MANAGER" },
                    { "f4bf842c-8164-4026-8c68-85b364e6cb07", "b1d66ada-5b41-46eb-9ab9-41c165e2ec0b", "Storageman", "STORAGEMAN" }
                });
        }
    }
}
