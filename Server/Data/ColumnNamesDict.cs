namespace Cargotruck.Server.Data
{
    //Export-import oszlop nevek ebből a dict-ből fognak majd jönni
    public static class ColumnNamesDict
    {
        public static readonly Dictionary<string, List<string>> tasksColumnNames = new()
        {
            { "hu"
                ,new (){
                    "Id", 
                    Shared.Resources.Resource.Partner,
                    Cargotruck.Shared.Resources.Resource.User_id,
                    Cargotruck.Shared.Resources.Resource.Partner,
                    Cargotruck.Shared.Resources.Resource.Description,
                    Cargotruck.Shared.Resources.Resource.Place_of_receipt,
                    Cargotruck.Shared.Resources.Resource.Time_of_receipt,
                    Cargotruck.Shared.Resources.Resource.Place_of_delivery,
                    Cargotruck.Shared.Resources.Resource.Time_of_delivery,
                    Cargotruck.Shared.Resources.Resource.Other_stops,
                    Cargotruck.Shared.Resources.Resource.Id_cargo,
                    Cargotruck.Shared.Resources.Resource.Storage_time,
                    Cargotruck.Shared.Resources.Resource.Completed,
                    Cargotruck.Shared.Resources.Resource.Completion_time,
                    Cargotruck.Shared.Resources.Resource.Time_of_delay,
                    Cargotruck.Shared.Resources.Resource.Payment,
                    Cargotruck.Shared.Resources.Resource.Final_Payment,
                    Cargotruck.Shared.Resources.Resource.Penalty,
                    Cargotruck.Shared.Resources.Resource.Date
                } 
            },
             { "en"
                ,new (){
                    "Id",
                    "User ID",
                    "Partner",
                    "Description",
                    "Place of receipt",
                    "Time of receipt",
                    "Place of delivery",
                    "Time of delivery",
                    "Other stops",
                    "Id cargo",
                    "Storage time",
                    "Completed",
                    "Completion time",
                    "Time of delay",
                    "Payment",
                    "Final Payment",
                    "Penalty",
                    "Date"
                }
            }
        };            
    }
}