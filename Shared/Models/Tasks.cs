using Cargotruck.Shared.Resources;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;



namespace Cargotruck.Shared.Models
{
    public class Tasks
    {
        [Required]
        [ForeignKey("Task_id")]
        public int Id { get; set; }
        public string? User_id { get; set; }
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
        [Range(typeof(DateTime), "1/2/2004", "3/4/2004",ErrorMessage = "Value for {0} must be between {1} and {2}")]
        [Display(Name = "Time_of_delivery", ResourceType = typeof(Resource))]
        public DateTime? Time_of_delivery { get; set; }
        [Display(Name = "other_stops", ResourceType = typeof(Resource))]
        public string? Other_stops { get; set; }
        [Display(Name = "Id_cargo", ResourceType = typeof(Resource))]
        public int? Id_cargo { get; set; }
        [Display(Name = "Storage_time", ResourceType = typeof(Resource))]
        public string? Storage_time { get; set; }
        [Display(Name = "completed", ResourceType = typeof(Resource))]
        public bool Completed { get; set; } = false;
        [Display(Name = "Completion_time", ResourceType = typeof(Resource))]
        public DateTime? Completion_time { get; set; }
        [Display(Name = "Time_of_delay", ResourceType = typeof(Resource))]
        public string? Time_of_delay { get; set; }
        [Display(Name = "Payment", ResourceType = typeof(Resource))]
        [Range(0, int.MaxValue, ErrorMessageResourceName = "OnlyPositive", ErrorMessageResourceType = typeof(Resource))]
        public int? Payment { get; set; }
        [Display(Name = "Final_Payment", ResourceType = typeof(Resource))]
        public int? Final_Payment { get; set; }
        [Display(Name = "Penalty", ResourceType = typeof(Resource))]
        [Range(0, int.MaxValue, ErrorMessageResourceName = "OnlyPositive", ErrorMessageResourceType = typeof(Resource))]
        public int? Penalty { get; set; }
        [Display(Name = "Date", ResourceType = typeof(Resource))]
        public DateTime Date { get; set; } = DateTime.Now;
    }

}
