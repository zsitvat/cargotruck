using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Cargotruck.Shared.Resources;

namespace Cargotruck.Shared.Models
{
    public class Monthly_expenses
    {
        [Required]
        [ForeignKey("Monthly_expense_id")]
        public int Id { get; set; }
        public string? User_id { get; set; }
        [Display(Name = "Task_id", ResourceType = typeof(Resource))]
        public int? Earning { get; set; } = 0;
        [Display(Name = "Profit", ResourceType = typeof(Resource))]
        public int? Profit { get; set; } = 0;
        [Display(Name = "Date", ResourceType = typeof(Resource))]
        public DateTime Date { get; set; } = DateTime.Now;

        public List<Monthly_expenses_tasks_expenses>? Monthly_expenses_tasks_expenses { get; set; }
    }
    public class Monthly_expenses_tasks_expenses
    {
        [Required]
        public int Id { get; set; }
        [Required]
        [ForeignKey("Id")]
        public int? Monthly_expense_id { get; set; }
        [ForeignKey("Id")]
        public int? Expense_id { get; set; }
        [ForeignKey("Id")]
        public int? Task_id { get; set; }
    }
}

