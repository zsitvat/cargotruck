using System;
using System.ComponentModel.DataAnnotations;
using Cargotruck.Shared.Resources;


namespace Cargotruck.Shared.Models
{
    public enum Status
    {
        delivering, 
        on_road, 
        garage, 
        under_repair, 
        loaned, 
        rented
    };
    public class Trucks
    {
        [Required]
        public int Id { get; set; }   
        public string? User_id { get; set; }
        [Required(ErrorMessageResourceName = "Error_vehicle_registration_number", ErrorMessageResourceType = typeof(Resource))]
        [Display(Name = "Vehicle_registration_number", ResourceType = typeof(Resource))]
        public string? Vehicle_registration_number { get; set; }
        [Display(Name = "Brand", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceName = "Error_brand", ErrorMessageResourceType = typeof(Resource))]
        public string? Brand { get; set; }
        [Display(Name = "Status", ResourceType = typeof(Resource))]
        public Status Status { get; set; }
        [Display(Name = "Road_id", ResourceType = typeof(Resource))]
        public int? Road_id { get; set; }
        [Display(Name = "Max_weight", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceName = "Error_max_weight", ErrorMessageResourceType = typeof(Resource))]
        public string? Max_weight { get; set; }
        [Display(Name = "Date", ResourceType = typeof(Resource))]
        public DateTime Date { get; set; } = DateTime.Now;
    }
}
