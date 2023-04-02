using Cargotruck.Shared.Resources;
using System.ComponentModel.DataAnnotations;


namespace Cargotruck.Shared.Model.Dto
{
    public class TruckDto
    {
        [Required]
        public int Id { get; set; }
        public string? UserId { get; set; }
        [Required(ErrorMessageResourceName = "Error_vehicle_registration_number", ErrorMessageResourceType = typeof(Resource))]
        [Display(Name = "Vehicle_registration_number", ResourceType = typeof(Resource))]
        public string? VehicleRegistrationNumber { get; set; }
        [Display(Name = "Brand", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceName = "Error_brand", ErrorMessageResourceType = typeof(Resource))]
        public string? Brand { get; set; }
        [Display(Name = "Status", ResourceType = typeof(Resource))]
        public Status Status { get; set; }
        [Display(Name = "Road_id", ResourceType = typeof(Resource))]
        public int? RoadId { get; set; }
        [Display(Name = "Max_weight", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceName = "Error_max_weight", ErrorMessageResourceType = typeof(Resource))]
        public int? MaxWeight { get; set; }
        [Display(Name = "Date", ResourceType = typeof(Resource))]
        public DateTime Date { get; set; } = DateTime.Now;
    }
}
