
using System;
using System.ComponentModel.DataAnnotations;
using Cargotruck.Shared.Resources;



namespace Cargotruck.Shared.Models
{
    public class Tasks
    {
        [Required]
        public int Id { get; set; }
        public long User_id { get; set; }
        [Required(ErrorMessageResourceName = "Error_partner", ErrorMessageResourceType = typeof(Resource))]
        [Display(Name = "Partner", ResourceType = typeof(Resource))]
        public string? Partner { get; set; }
        [Display(Name = "Description", ResourceType = typeof(Resource))] 
        public string? Description { get; set; }
        [Required(ErrorMessageResourceName = "Error_place_of_receipt", ErrorMessageResourceType = typeof(Resource))]
        [Display(Name = "Place_of_receipt", ResourceType = typeof(Resource))]
        public string? Place_of_receipt { get; set; }
        [Required(ErrorMessageResourceName = "Error_time_of_receipt", ErrorMessageResourceType = typeof(Resource))]
        [Display(Name = "Time_of_receipt", ResourceType = typeof(Resource))]
        public DateTime? Time_of_receipt { get; set; }
        [Required(ErrorMessageResourceName = "Error_place_of_delivery", ErrorMessageResourceType = typeof(Resource))]
        [Display(Name = "Place_of_delivery", ResourceType = typeof(Resource))]
        public string? Place_of_delivery { get; set; }
        [Required(ErrorMessageResourceName = "Error_time_of_delivery", ErrorMessageResourceType = typeof(Resource))]
        [Display(Name = "Time_of_delivery", ResourceType = typeof(Resource))]
        public DateTime? Time_of_delivery { get; set; }
        [Display(Name = "Other_stops", ResourceType = typeof(Resource))]
        public string? Other_stops { get; set; }
        [Display(Name = "Id_cargo", ResourceType = typeof(Resource))]
        public string? Id_cargo { get; set; }
        [Display(Name = "Storage_time", ResourceType = typeof(Resource))]
        public string? Storage_time { get; set; }
        [Display(Name = "Completed", ResourceType = typeof(Resource))]
        public bool Completed { get; set; } = false;
        [Display(Name = "Completion_time", ResourceType = typeof(Resource))]
        public DateTime? Completion_time { get; set; }
        [Display(Name = "Time_of_delay", ResourceType = typeof(Resource))]
        public string? Time_of_delay { get; set; }
        [Display(Name = "Payment", ResourceType = typeof(Resource))]
        public int? Payment { get; set; }
        [Display(Name = "Final_Payment", ResourceType = typeof(Resource))]
        public int? Final_Payment { get; set; }
        [Display(Name = "Penalty", ResourceType = typeof(Resource))]
        public int? Penalty { get; set; }
        [Display(Name = "Date", ResourceType = typeof(Resource))]
        public DateTime Date { get; set; } = DateTime.Now;
    }

}
