using App.Resources;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace App.Models
{
    public class Monthly_expenses
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public int User_id { get; set; }
        [Display(Name = "Task_id", ResourceType = typeof(Resource))]
        public List<int> Task_id { get; set; }
        [Display(Name = "Expenses_id", ResourceType = typeof(Resource))]
        public List<int> Expenses { get; set; }
        [Display(Name = "Earning", ResourceType = typeof(Resource))]
        public int Earning { get; set; }
        [Display(Name = "Profit", ResourceType = typeof(Resource))]
        public int Profit { get; set; }
        [Display(Name = "Date", ResourceType = typeof(Resource))]
        public DateTime Date { get; set; } = DateTime.Now;
    }
}
