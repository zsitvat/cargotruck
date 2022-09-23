using System;
using System.ComponentModel.DataAnnotations;

namespace App.Models
{
    public enum status
    {
        delivering,on_road, garage, under_repair, loaned
    }
    public class Trucks
    {
        [Required]
        public int Id { get; set; }
        [Required]
        [Display(Name = "Rendszám")]
        public int Vehicle_registration_number { get; set; }
        [Required]
        public int User_id { get; set; }
        public string Brand { get; set; }
        [Display(Name = "Státusz")]
        public string Status { get; set; }
        [Display(Name = "Út azonosító")]
        public int Road_id { get; set; }
        [Display(Name = "Terhelés max súlya")]
        public string Max_weight { get; set; }
        [Display(Name = "Dátum")]
        public DateTime Date { get; set; } = DateTime.Now;
    }
}
