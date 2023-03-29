using Cargotruck.Server.Services.Interfaces;
using Cargotruck.Shared.Resources;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace Cargotruck.Server.Services
{
    public class ColumnNamesService : IColumnNamesService
    {
        private readonly List<string> TasksColumnNames;
        private readonly List<string> CargoesColumnNames;
        private readonly List<string> ExpensesColumnNames;
        private readonly List<string> MonthlyExpensesColumnNames;
        private readonly List<string> RoadsColumnNames;
        private readonly List<string> TrucksColumnNames;
        private readonly List<string> WarehousessColumnNames;

        public ColumnNamesService()
        {
            TasksColumnNames = new List<string>
            {
                "Id",
                "Partner",
                "Description",
                "Place_of_receipt",
                "Time_of_receipt",
                "Place_of_delivery",
                "Time_of_delivery",
                "Other_stops",
                "Id_cargo",
                "Storage_time",
                "Completed",
                "Completion_time",
                "Time_of_delay",
                "Payment",
                "Final_Payment",
                "Penalty",
                "Date"
            };

            CargoesColumnNames = new List<string>
            {
                "Id",
                "Task_id",
                "Weight",
                "Description",
                "Delivery_requirements",
                "Vehicle_registration_number",
                "Warehouse_id",
                "Warehouse_section",
                "Storage_starting_time",
                "Date"
            };
            ExpensesColumnNames = new List<string>
            {
                 "Id",
                 "Type",
                 "Type_id",
                 "Fuel",
                 "Road_fees",
                 "Penalty",
                 "Driver_spending",
                 "Driver_salary",
                 "Repair_cost",
                 "Repair_description",
                 "Cost_of_storage",
                 "other",
                 "Total_amount",
                 "Date"
            };
            MonthlyExpensesColumnNames = new List<string>
            {
                "Id",
                "Month",
                "Earning",
                "Expense",
                "Profit",
                "Expense_id",
                "Task_id",
                "Date"
            };
            RoadsColumnNames = new List<string>
            {
                "Id",
                "Task_id",
                "Vehicle_registration_number",
                "Id_cargo",
                "Purpose_of_the_trip",
                "Starting_date",
                "Ending_date",
                "Starting_place",
                "Ending_place",
                "Direction",
                "Distance",
                "Fuel",
                "Expenses_id",
                "Date"
            };
            TrucksColumnNames = new List<string>
            {
                 "Id",
                 "Vehicle_registration_number",
                 "Brand",
                 "Status",
                 "Road_id",
                 "Max_weight",
                 "Date"
            };
            WarehousessColumnNames = new List<string>
            {
                  "Id",
                 "Address",
                 "Owner",
                 "Cargo_id",
                 "Date"
            };
        }

        public List<string> GetTasksColumnNames()
        {
            return TasksColumnNames;
        }

        public List<string> GetCargoesColumnNames()
        {
            return CargoesColumnNames;
        }

        public List<string> GetExpensesColumnNames()
        {
            return ExpensesColumnNames;
        }

        public List<string> GetMonthlyExpensesColumnNames()
        {
            return MonthlyExpensesColumnNames;
        }

        public List<string> GetRoadsColumnNames()
        {
            return RoadsColumnNames;
        }

        public List<string> GetTrucksColumnNames()
        {
            return TrucksColumnNames;
        }

        public List<string> GetWarehousesColumnNames()
        {
            return WarehousessColumnNames;
        }
    }
}