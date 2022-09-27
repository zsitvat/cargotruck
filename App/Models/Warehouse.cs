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
        [Required]
        [Display(Name = "Raktár címe")]
        public string Address { get; set; }
        [Display(Name = "Tulajdonos")]
        public string Owner { get; set; }
        [Display(Name = "Rakomány azonosító")]
        public List<int> Cargo_id { get; set; }
        [Display(Name = "Dátum")]
        public DateTime Date { get; set; } = DateTime.Now;
    }
}
