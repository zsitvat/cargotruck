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
        private static readonly List<string> MonthlyColumnNames;
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
            };
            ExpensesColumnNames = new List<string>
            {
                "Id",
            };
            MonthlyColumnNames = new List<string>
            {
                "Id",
            };
            RoadsColumnNames = new List<string>
            {
                "Id",
            };
            TrucksColumnNames = new List<string>
            {
                "Id",
            };
            WarehousessColumnNames = new List<string>
            {
                "Id",
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
            return MonthlyColumnNames;
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