using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cargotruck.Server.Migrations
{
    public partial class _0403 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Monthly_expenses_tasks_expenses_Monthly_Expenses_MonthlyExpenseId",
                table: "Monthly_expenses_tasks_expenses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Monthly_expenses_tasks_expenses",
                table: "Monthly_expenses_tasks_expenses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Monthly_Expenses",
                table: "Monthly_Expenses");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "15cbed15-3054-40f5-8eee-203909890aba");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "26204a4f-099a-4601-8c46-1c43221c04ec");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "50014528-fba2-4a5b-832c-090f49d773c3");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "8134658b-2928-463c-b3dc-4dc86fd7859d");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a186fe2d-7290-49c2-987d-f2c0714909e7");

            migrationBuilder.RenameTable(
                name: "Monthly_expenses_tasks_expenses",
                newName: "MonthlyExpensesTasksExpenses");

            migrationBuilder.RenameTable(
                name: "Monthly_Expenses",
                newName: "MonthlyExpenses");

            migrationBuilder.RenameIndex(
                name: "IX_Monthly_expenses_tasks_expenses_MonthlyExpenseId",
                table: "MonthlyExpensesTasksExpenses",
                newName: "IX_MonthlyExpensesTasksExpenses_MonthlyExpenseId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MonthlyExpensesTasksExpenses",
                table: "MonthlyExpensesTasksExpenses",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MonthlyExpenses",
                table: "MonthlyExpenses",
                column: "Id");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "143eceb5-088f-4468-8680-edb754fa574f", "9fc33a7a-0c70-44eb-aa24-b718ecf99152", "Storageman", "STORAGEMAN" },
                    { "28d0f1a5-d87d-4f1a-b14e-c3fbe990e627", "582e4cb5-7169-4399-8132-b5cc470ef7a6", "Accountant", "ACCOUNTANT" },
                    { "f0648ad6-bb28-4346-804e-382d26c72b42", "ecdc9855-762e-4417-8654-43debeccf5a1", "Admin", "ADMIN" },
                    { "f12f78a3-6175-4b67-90cf-ac3b3ef2a9ef", "2d8fbc9f-2092-4bfc-a3a0-277dd9f8a6fd", "User", "USER" },
                    { "fbddcd40-db1f-4f29-8c45-36915a2b51a6", "6932f7bf-c17e-4494-8d3f-eda535f95521", "Driver", "DRIVER" }
                });

            migrationBuilder.AddForeignKey(
                name: "FK_MonthlyExpensesTasksExpenses_MonthlyExpenses_MonthlyExpenseId",
                table: "MonthlyExpensesTasksExpenses",
                column: "MonthlyExpenseId",
                principalTable: "MonthlyExpenses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MonthlyExpensesTasksExpenses_MonthlyExpenses_MonthlyExpenseId",
                table: "MonthlyExpensesTasksExpenses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MonthlyExpensesTasksExpenses",
                table: "MonthlyExpensesTasksExpenses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MonthlyExpenses",
                table: "MonthlyExpenses");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "143eceb5-088f-4468-8680-edb754fa574f");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "28d0f1a5-d87d-4f1a-b14e-c3fbe990e627");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "f0648ad6-bb28-4346-804e-382d26c72b42");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "f12f78a3-6175-4b67-90cf-ac3b3ef2a9ef");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "fbddcd40-db1f-4f29-8c45-36915a2b51a6");

            migrationBuilder.RenameTable(
                name: "MonthlyExpensesTasksExpenses",
                newName: "Monthly_expenses_tasks_expenses");

            migrationBuilder.RenameTable(
                name: "MonthlyExpenses",
                newName: "Monthly_Expenses");

            migrationBuilder.RenameIndex(
                name: "IX_MonthlyExpensesTasksExpenses_MonthlyExpenseId",
                table: "Monthly_expenses_tasks_expenses",
                newName: "IX_Monthly_expenses_tasks_expenses_MonthlyExpenseId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Monthly_expenses_tasks_expenses",
                table: "Monthly_expenses_tasks_expenses",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Monthly_Expenses",
                table: "Monthly_Expenses",
                column: "Id");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "15cbed15-3054-40f5-8eee-203909890aba", "4305171c-0e22-4b73-8010-3cffcff8c97f", "Admin", "ADMIN" },
                    { "26204a4f-099a-4601-8c46-1c43221c04ec", "c33ab5dc-1545-4b1b-aef8-de04768e16d5", "Storageman", "STORAGEMAN" },
                    { "50014528-fba2-4a5b-832c-090f49d773c3", "0258a410-0641-4644-bf4e-d7a5a3fdf867", "Driver", "DRIVER" },
                    { "8134658b-2928-463c-b3dc-4dc86fd7859d", "5987292f-5433-4971-8897-cdbd74afa185", "User", "USER" },
                    { "a186fe2d-7290-49c2-987d-f2c0714909e7", "fa884d7f-c477-45ae-8c62-b62019ba9e1e", "Accountant", "ACCOUNTANT" }
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Monthly_expenses_tasks_expenses_Monthly_Expenses_MonthlyExpenseId",
                table: "Monthly_expenses_tasks_expenses",
                column: "MonthlyExpenseId",
                principalTable: "Monthly_Expenses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
