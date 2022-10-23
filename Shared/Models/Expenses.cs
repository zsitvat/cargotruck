using Cargotruck.Shared.Resources;
using System;
using System.ComponentModel.DataAnnotations;

namespace Cargotruck.Shared.Models
{
    public class Expenses
    {
        [Required]
        public long Id { get; set; }
        [Required]
        public long User_id { get; set; }
        [Required(ErrorMessageResourceName = "Error_type", ErrorMessageResourceType = typeof(Resource))]
        [Display(Name = "Type", ResourceType = typeof(Resource))]
        public string Type { get; set; }
        [Required(ErrorMessageResourceName = "Error_type_id", ErrorMessageResourceType = typeof(Resource))]
        [Display(Name = "Type_id", ResourceType = typeof(Resource))]
        public int Type_id { get; set; }
        [Display(Name = "Fuel", ResourceType = typeof(Resource))]
        public int Fuel { get; set; }
        [Display(Name = "Road_fees", ResourceType = typeof(Resource))]
        public int Road_fees { get; set; }
        [Display(Name = "Penalty_expenses", ResourceType = typeof(Resource))]
        public int Penalty { get; set; }
        [Display(Name = "Driver_spending", ResourceType = typeof(Resource))]
        public int Driver_spending { get; set; }
        [Display(Name = "Driver_salary", ResourceType = typeof(Resource))]
        public int Driver_salary { get; set; }
        [Display(Name = "Repair_cost", ResourceType = typeof(Resource))]
        public int Repair_cost { get; set; }
        [Display(Name = "Repair_description", ResourceType = typeof(Resource))]
        public string Repair_description { get; set; }
        [Display(Name = "Cost_of_storage", ResourceType = typeof(Resource))]
        public int Cost_of_storage { get; set; }
        [Display(Name = "Other", ResourceType = typeof(Resource))]
        public int Other { get; set; }
        [Display(Name = "Date", ResourceType = typeof(Resource))]
        public DateTime Date { get; set; } = DateTime.Now;
    }
}
