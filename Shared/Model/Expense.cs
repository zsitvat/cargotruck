using Cargotruck.Shared.Resources;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cargotruck.Shared.Model
{
    public class Expense
    {
        [Required]
        public int Id { get; set; }
        public string? UserId { get; set; }
        [Required(ErrorMessageResourceName = "Error_type", ErrorMessageResourceType = typeof(Resource))]
        [Display(Name = "Type", ResourceType = typeof(Resource))]
        public Type? Type { get; set; }
        [Display(Name = "TypeId", ResourceType = typeof(Resource))]
        public int? TypeId { get; set; }
        [Display(Name = "Fuel", ResourceType = typeof(Resource))]
        [Range(0, int.MaxValue, ErrorMessageResourceName = "OnlyPositive", ErrorMessageResourceType = typeof(Resource))]
        public int? Fuel { get; set; }
        [Display(Name = "Road_fees", ResourceType = typeof(Resource))]
        [Range(0, int.MaxValue, ErrorMessageResourceName = "OnlyPositive", ErrorMessageResourceType = typeof(Resource))]
        public int? RoadFees { get; set; }
        [Display(Name = "Penalty_expenses", ResourceType = typeof(Resource))]
        [Range(0, int.MaxValue, ErrorMessageResourceName = "OnlyPositive", ErrorMessageResourceType = typeof(Resource))]
        public int? Penalty { get; set; }
        [Display(Name = "Driver_spending", ResourceType = typeof(Resource))]
        [Range(0, int.MaxValue, ErrorMessageResourceName = "OnlyPositive", ErrorMessageResourceType = typeof(Resource))]
        public int? DriverSpending { get; set; }
        [Display(Name = "Driver_salary", ResourceType = typeof(Resource))]
        [Range(0, int.MaxValue, ErrorMessageResourceName = "OnlyPositive", ErrorMessageResourceType = typeof(Resource))]
        public int? DriverSalary { get; set; }
        [Display(Name = "Repair_cost", ResourceType = typeof(Resource))]
        [Range(0, int.MaxValue, ErrorMessageResourceName = "OnlyPositive", ErrorMessageResourceType = typeof(Resource))]
        public int? RepairCost { get; set; }
        [Display(Name = "Repair_description", ResourceType = typeof(Resource))]
        public string? RepairDescription { get; set; }
        [Display(Name = "Cost_of_storage", ResourceType = typeof(Resource))]
        [Range(0, int.MaxValue, ErrorMessageResourceName = "OnlyPositive", ErrorMessageResourceType = typeof(Resource))]
        public int? CostOfStorage { get; set; }
        [Display(Name = "Other", ResourceType = typeof(Resource))]
        [Range(0, int.MaxValue, ErrorMessageResourceName = "OnlyPositive", ErrorMessageResourceType = typeof(Resource))]
        public int? Other { get; set; }
        [Display(Name = "Total_amount", ResourceType = typeof(Resource))]
        [Range(0, int.MaxValue, ErrorMessageResourceName = "OnlyPositive", ErrorMessageResourceType = typeof(Resource))]
        public int? TotalAmount { get; set; }
        [Display(Name = "Date", ResourceType = typeof(Resource))]
        public DateTime Date { get; set; } = DateTime.Now;
    }
    public enum Type
    {
        task, repair, storage, salary, othertype
    }
}
