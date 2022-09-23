using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.ComponentModel.DataAnnotations;

namespace App.Models
{
    public class Cargoes
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public int User_id { get; set; }
        [Required]
        [Display(Name = "Megbízás azonosító")]
        public int Task_id { get; set; }
        [Required]
        [Display(Name = "Súly")]
        public string Weight{ get; set; }
        [Display(Name = "Leírás")]
        public string Description { get; set; }
        [Display(Name = "Szállítási követelmények")]
        public string Delivery_requirements { get; set; }
        [Display(Name = "Jármű Rendszáma")]
        public string Vehicle_registration_number { get; set; }
        [Display(Name = "Raktár azonosító")]
        public int Warehouse_id { get; set; }
        [Display(Name = "Raktár szekció")]
        public string Warehouse_section { get; set; }
        [Display(Name = "Raktározás kezdete")]
        public DateTime Storage_starting_time { get; set; }
        [Display(Name = "Raktározás költsége")]
        public int Cost_of_storage { get; set; }
        [Display(Name = "Dátum")]
        public DateTime Date { get; set; } = DateTime.Now;

    }
}
