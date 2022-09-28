using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using App.Resources;

namespace App.Models
{
    public class Warehouse
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public int User_id { get; set; }
        [Required(ErrorMessageResourceName = "Error_Warehouse_address", ErrorMessageResourceType = typeof(Resource))]
        [Display(Name = "Warehouse_address", ResourceType = typeof(Resource))]
        public string Address { get; set; }
        [Display(Name = "Warehouse_owner")] 
        [Required(ErrorMessageResourceName = "Error_Warehouse_owner", ErrorMessageResourceType = typeof(Resource))]
        public string Owner { get; set; }
        [Display(Name = "Id_cargo", ResourceType = typeof(Resource))]
        public List<int> Cargo_id { get; set; }
        [Display(Name = "Date", ResourceType = typeof(Resource))]
        public DateTime Date { get; set; } = DateTime.Now;
    }
}
