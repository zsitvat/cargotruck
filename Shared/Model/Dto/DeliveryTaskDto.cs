using Cargotruck.Shared.Resources;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cargotruck.Shared.Model.Dto
{
    public class DeliveryTaskDto
    {
        [Required]
        public int Id { get; set; }
        public string? UserId { get; set; }
        [Required(ErrorMessageResourceName = "Error_partner", ErrorMessageResourceType = typeof(Resource))]
        [Display(Name = "Partner", ResourceType = typeof(Resource))]
        public string? Partner { get; set; }
        [Display(Name = "Description", ResourceType = typeof(Resource))]
        public string? Description { get; set; }
        [Required(ErrorMessageResourceName = "Error_place_of_receipt", ErrorMessageResourceType = typeof(Resource))]
        [Display(Name = "Place_of_receipt", ResourceType = typeof(Resource))]
        public string? PlaceOfReceipt { get; set; }
        [Required(ErrorMessageResourceName = "Error_time_of_receipt", ErrorMessageResourceType = typeof(Resource))]
        [Display(Name = "Time_of_receipt", ResourceType = typeof(Resource))]
        public DateTime? TimeOfReceipt { get; set; }
        [Required(ErrorMessageResourceName = "Error_place_of_delivery", ErrorMessageResourceType = typeof(Resource))]
        [Display(Name = "Place_of_delivery", ResourceType = typeof(Resource))]
        public string? PlaceOfDelivery { get; set; }
        [Required(ErrorMessageResourceName = "Error_time_of_delivery", ErrorMessageResourceType = typeof(Resource))]
        [Display(Name = "Time_of_delivery", ResourceType = typeof(Resource))]
        public DateTime? TimeOfDelivery { get; set; }
        [Display(Name = "Other_stops", ResourceType = typeof(Resource))]
        public string? OtherStops { get; set; }
        [Display(Name = "Storage_time", ResourceType = typeof(Resource))]
        public string? StorageTime { get; set; }
        [Display(Name = "Completed", ResourceType = typeof(Resource))]
        public bool Completed { get; set; } = false;
        [Display(Name = "Completion_time", ResourceType = typeof(Resource))]
        public DateTime? CompletionTime { get; set; }
        [Display(Name = "Time_of_delay", ResourceType = typeof(Resource))]
        public string? TimeOfDelay { get; set; }
        [Display(Name = "Payment", ResourceType = typeof(Resource))]
        [Range(0, long.MaxValue, ErrorMessageResourceName = "OnlyPositive", ErrorMessageResourceType = typeof(Resource))]
        public long? Payment { get; set; }
        [Display(Name = "Final_payment", ResourceType = typeof(Resource))]
        [Range(long.MinValue, long.MaxValue, ErrorMessageResourceName = "OnlyPositive", ErrorMessageResourceType = typeof(Resource))]
        public long? FinalPayment { get; set; }
        [Display(Name = "Penalty", ResourceType = typeof(Resource))]
        [Range(0, long.MaxValue, ErrorMessageResourceName = "OnlyPositive", ErrorMessageResourceType = typeof(Resource))]
        public long? Penalty { get; set; }
        [Display(Name = "Date", ResourceType = typeof(Resource))]
        public DateTime Date { get; set; } = DateTime.Now;
        [Display(Name = "Cargo_id", ResourceType = typeof(Resource))]
        public int? CargoId { get; set; }
    }

}
