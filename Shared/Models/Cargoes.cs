using Cargotruck.Shared.Resources;
using System;
using System.ComponentModel.DataAnnotations;

namespace Cargotruck.Shared.Models
{
    public class Cargoes
    {
        [Required]
        public int Id { get; set; }
        public string? User_id { get; set; }
        [Required(ErrorMessageResourceName = "Error_task_id", ErrorMessageResourceType = typeof(Resource))]
        [Display(Name = "Task_id", ResourceType = typeof(Resource))]
        public int Task_id { get; set; }
        [Required(ErrorMessageResourceName = "Error_weight", ErrorMessageResourceType = typeof(Resource))]
        [Display(Name = "Weight", ResourceType = typeof(Resource))]
        public string Weight { get; set; } = String.Empty;
        [Display(Name = "Description", ResourceType = typeof(Resource))]
        public string? Description { get; set; }
        [Display(Name = "Shipping_requirements", ResourceType = typeof(Resource))]
        public string? Delivery_requirements { get; set; }
        [Display(Name = "Vehicle_registration_number", ResourceType = typeof(Resource))]
        public string? Vehicle_registration_number { get; set; }
        [Display(Name = "Warehouse_id", ResourceType = typeof(Resource))]
        public int? Warehouse_id { get; set; }
        [Display(Name = "Section", ResourceType = typeof(Resource))]
        public string? Warehouse_section { get; set; }
        [Display(Name = "Storage_starting_time", ResourceType = typeof(Resource))]
        public DateTime? Storage_starting_time { get; set; }
        [Display(Name = "Cost_of_storage", ResourceType = typeof(Resource))]
        public DateTime Date { get; set; } = DateTime.Now;

    }
}
