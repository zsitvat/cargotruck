﻿using Cargotruck.Shared.Resources;
using System.ComponentModel.DataAnnotations;

namespace Cargotruck.Shared.Models
{
    public class Roads
    {
        [Required]
        public int Id { get; set; }
        public string? User_id { get; set; }
        [Display(Name = "Task_id", ResourceType = typeof(Resource))]
        public int? Task_id { get; set; }
        [Display(Name = "Vehicle_registration_number", ResourceType = typeof(Resource))]
        public string? Vehicle_registration_number { get; set; }
        [Display(Name = "Id_cargo", ResourceType = typeof(Resource))]
        public int? Id_cargo { get; set; }
        [Required(ErrorMessageResourceName = "Error_purpose_of_the_trip", ErrorMessageResourceType = typeof(Resource))]
        [Display(Name = "Purpose_of_the_trip", ResourceType = typeof(Resource))]
        public string? Purpose_of_the_trip { get; set; }
        [Required(ErrorMessageResourceName = "Error_starting_date", ErrorMessageResourceType = typeof(Resource))]
        [Display(Name = "Starting_date", ResourceType = typeof(Resource))]
        public DateTime? Starting_date { get; set; }
        [Display(Name = "Ending_date", ResourceType = typeof(Resource))]
        public DateTime? Ending_date { get; set; }
        [Required(ErrorMessageResourceName = "Error_starting_place", ErrorMessageResourceType = typeof(Resource))]
        [Display(Name = "Starting_place", ResourceType = typeof(Resource))]
        public string? Starting_place { get; set; }
        [Display(Name = "Ending_place", ResourceType = typeof(Resource))]
        public string? Ending_place { get; set; }
        [Required(ErrorMessageResourceName = "Error_direction", ErrorMessageResourceType = typeof(Resource))]
        [Display(Name = "Direction", ResourceType = typeof(Resource))]
        public string? Direction { get; set; }
        [Display(Name = "Fuel", ResourceType = typeof(Resource))]
        public string? Fuel { get; set; }
        [Display(Name = "Expenses_id", ResourceType = typeof(Resource))]
        public int? Expenses_id { get; set; }
        [Display(Name = "Date", ResourceType = typeof(Resource))]
        public DateTime Date { get; set; } = DateTime.Now;

    }
}
