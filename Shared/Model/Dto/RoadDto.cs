using Cargotruck.Shared.Resources;
using System.ComponentModel.DataAnnotations;

namespace Cargotruck.Shared.Model.Dto
{
    public class RoadDto
    {
        [Required]
        public int Id { get; set; }
        public string? UserId { get; set; }
        [Display(Name = "Task_id", ResourceType = typeof(Resource))]
        public int? TaskId { get; set; }
        [Display(Name = "Vehicle_registration_number", ResourceType = typeof(Resource))]
        public string? VehicleRegistrationNumber { get; set; }
        [Display(Name = "Cargo_id", ResourceType = typeof(Resource))]
        public int? CargoId { get; set; }
        [Required(ErrorMessageResourceName = "Error_purpose_of_the_trip", ErrorMessageResourceType = typeof(Resource))]
        [Display(Name = "Purpose_of_the_trip", ResourceType = typeof(Resource))]
        public string? PurposeOfTheTrip { get; set; }
        [Required(ErrorMessageResourceName = "Error_starting_date", ErrorMessageResourceType = typeof(Resource))]
        [Display(Name = "Starting_date", ResourceType = typeof(Resource))]
        public DateTime? StartingDate { get; set; }
        [Display(Name = "Ending_date", ResourceType = typeof(Resource))]
        public DateTime? EndingDate { get; set; }
        [Required(ErrorMessageResourceName = "Error_starting_place", ErrorMessageResourceType = typeof(Resource))]
        [Display(Name = "Starting_place", ResourceType = typeof(Resource))]
        public string? StartingPlace { get; set; }
    
        [Display(Name = "Ending_place", ResourceType = typeof(Resource))]
        public string? EndingPlace { get; set; }
        [Required(ErrorMessageResourceName = "Error_direction", ErrorMessageResourceType = typeof(Resource))]
        [Display(Name = "Direction", ResourceType = typeof(Resource))]
        public string? Direction { get; set; }    
        [Required(ErrorMessageResourceName = "Error_distance", ErrorMessageResourceType = typeof(Resource))]
        [Display(Name = "Distance", ResourceType = typeof(Resource))]
        [Range(0, int.MaxValue, ErrorMessageResourceName = "OnlyPositive", ErrorMessageResourceType = typeof(Resource))]
        public int? Distance { get; set; }
        [Display(Name = "Fuel", ResourceType = typeof(Resource))]
        [Range(0, int.MaxValue, ErrorMessageResourceName = "OnlyPositive", ErrorMessageResourceType = typeof(Resource))]
        public int? Fuel { get; set; }
        [Display(Name = "Expenses_id", ResourceType = typeof(Resource))]
        public int? ExpensesId { get; set; }
        [Display(Name = "Date", ResourceType = typeof(Resource))]
        public DateTime Date { get; set; } = DateTime.Now;

    }
}
