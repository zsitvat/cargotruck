using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Cargotruck.Shared.Resources;

namespace Cargotruck.Shared.Models
{
    public class Monthly_expenses
    {
        [Required]
        public int Id { get; set; }
        public string? User_id { get; set; }
        [Display(Name = "Task_id", ResourceType = typeof(Resource))]
        public List<Tasks>? Task_id { get; set; }
        [Display(Name = "Expenses_id", ResourceType = typeof(Resource))]
        public List<Expenses>? Expenses { get; set; }
        [Display(Name = "Earning", ResourceType = typeof(Resource))]
        public int Earning { get; set; } = 0;
        [Display(Name = "Profit", ResourceType = typeof(Resource))]
        public int Profit { get; set; } = 0;
        [Display(Name = "Date", ResourceType = typeof(Resource))]
        public DateTime Date { get; set; } = DateTime.Now;      
    }   
}

