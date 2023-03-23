using Cargotruck.Shared.Resources;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace Cargotruck.Server.Data
{
    public class ColumnNames
    {
        private static readonly List<string> TasksColumnNames;
        private static readonly List<string> CargoesColumnNames;
        private static readonly List<string> ExpensesColumnNames;
        private static readonly List<string> MonthlyExpensesColumnNames;
        private static readonly List<string> RoadsColumnNames;
        private static readonly List<string> TrucksColumnNames;
        private static readonly List<string> WarehousessColumnNames;

        static ColumnNames()
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
                "User_id",
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
                 "User_id",
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
                "User_id",
                "Earning",
                "Expense",
                "vProfit",
                "Expense_id",
                "Task_id",
                "Date"
            };
            RoadsColumnNames = new List<string>
            {
                 "Id",
                "User_id",
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
                 "User_id",
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
                "User_id",
                 "Address",
                 "Owner",
                 "Cargo_id",
                 "Date"
            };
        }

        public static List<string> GetTaskscolumnNames()
        {
            return TasksColumnNames;
        }

        public static List<string> GetCargoescolumnNames()
        {
            return CargoesColumnNames;
        }

        public static List<string> GetExpensescolumnNames()
        {
            return ExpensesColumnNames;
        }

        public static List<string> GetMonthlyExpensescolumnNames()
        {
            return MonthlyExpensesColumnNames;
        }

        public static List<string> GetRoadscolumnNames()
        {
            return RoadsColumnNames;
        }

        public static List<string> GetTruckscolumnNames()
        {
            return TrucksColumnNames;
        }

        public static List<string> GetWarehousescolumnNames()
        {
            return WarehousessColumnNames;
        }
    }
}