using Cargotruck.Shared.Resources;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cargotruck.Shared.Models
{
    public class Expenses
    {
        [Required]
        [ForeignKey("Expense_id")]
        public int Id { get; set; }
        public string? User_id { get; set; }
        [Required(ErrorMessageResourceName = "Error_type", ErrorMessageResourceType = typeof(Resource))]
        [Display(Name = "Type", ResourceType = typeof(Resource))]
        public Type? Type { get; set; }
        [Display(Name = "Type_id", ResourceType = typeof(Resource))]
        public int? Type_id { get; set; }
        [Display(Name = "Fuel", ResourceType = typeof(Resource))]
        [Range(0, int.MaxValue, ErrorMessageResourceName = "OnlyPositive", ErrorMessageResourceType = typeof(Resource))]
        public int? Fuel { get; set; }
        [Display(Name = "Road_fees", ResourceType = typeof(Resource))]
        [Range(0, int.MaxValue, ErrorMessageResourceName = "OnlyPositive", ErrorMessageResourceType = typeof(Resource))]
        public int? Road_fees { get; set; }
        [Display(Name = "Penalty_expenses", ResourceType = typeof(Resource))]
        [Range(0, int.MaxValue, ErrorMessageResourceName = "OnlyPositive", ErrorMessageResourceType = typeof(Resource))]
        public int? Penalty { get; set; }
        [Display(Name = "Driver_spending", ResourceType = typeof(Resource))]
        [Range(0, int.MaxValue, ErrorMessageResourceName = "OnlyPositive", ErrorMessageResourceType = typeof(Resource))]
        public int? Driver_spending { get; set; }
        [Display(Name = "Driver_salary", ResourceType = typeof(Resource))]
        [Range(0, int.MaxValue, ErrorMessageResourceName = "OnlyPositive", ErrorMessageResourceType = typeof(Resource))]
        public int? Driver_salary { get; set; }
        [Display(Name = "Repair_cost", ResourceType = typeof(Resource))]
        [Range(0, int.MaxValue, ErrorMessageResourceName = "OnlyPositive", ErrorMessageResourceType = typeof(Resource))]
        public int? Repair_cost { get; set; }
        [Display(Name = "Repair_description", ResourceType = typeof(Resource))]
        public string? Repair_description { get; set; }
        [Display(Name = "Cost_of_storage", ResourceType = typeof(Resource))]
        [Range(0, int.MaxValue, ErrorMessageResourceName = "OnlyPositive", ErrorMessageResourceType = typeof(Resource))]
        public int? Cost_of_storage { get; set; }
        [Display(Name = "other", ResourceType = typeof(Resource))]
        [Range(0, int.MaxValue, ErrorMessageResourceName = "OnlyPositive", ErrorMessageResourceType = typeof(Resource))]
        public int? other { get; set; }
        [Display(Name = "Date", ResourceType = typeof(Resource))]
        public DateTime Date { get; set; } = DateTime.Now;
    }

    public enum Type
    {
        task, repair, storage, salary, other
    }
}
