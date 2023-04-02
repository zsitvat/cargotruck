using Cargotruck.Shared.Resources;
using System.ComponentModel.DataAnnotations;

namespace Cargotruck.Shared.Model.Dto
{
    public class CargoDto
    {
        [Required]
        public int Id { get; set; }
        public string? UserId { get; set; }
        [Required(ErrorMessageResourceName = "Error_weight", ErrorMessageResourceType = typeof(Resource))]
        [Display(Name = "Weight", ResourceType = typeof(Resource))]
        public int? Weight { get; set; }
        [Display(Name = "Description", ResourceType = typeof(Resource))]
        public string? Description { get; set; }
        [Display(Name = "Delivery_requirements", ResourceType = typeof(Resource))]
        public string? DeliveryRequirements { get; set; }
        [Display(Name = "Vehicle_registration_number", ResourceType = typeof(Resource))]
        public string? VehicleRegistrationNumber { get; set; }
        [Display(Name = "Warehouse_id", ResourceType = typeof(Resource))]
        public int? WarehouseId { get; set; }
        [Display(Name = "Section", ResourceType = typeof(Resource))]
        public string? WarehouseSection { get; set; }
        [Display(Name = "Storage_starting_time", ResourceType = typeof(Resource))]
        public DateTime? StorageStartingTime { get; set; }
        [Display(Name = "Date", ResourceType = typeof(Resource))]
        public DateTime Date { get; set; } = DateTime.Now;

        [Display(Name = "Task_id", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceName = "Error_task_id", ErrorMessageResourceType = typeof(Resource))]
        public int? TaskId { get; set; }
    }
}
