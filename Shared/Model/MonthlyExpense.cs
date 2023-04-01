using Cargotruck.Shared.Resources;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cargotruck.Shared.Model
{
    public class MonthlyExpense
    {
        [Required]
        [Key]
        public int Monthly_expense_id { get; set; }
        public string? User_id { get; set; }
        [Display(Name = "Earning", ResourceType = typeof(Resource))]
        [Range(0, int.MaxValue, ErrorMessageResourceName = "OnlyPositive", ErrorMessageResourceType = typeof(Resource))]
        public long? Earning { get; set; } = 0;
        [Display(Name = "Expense", ResourceType = typeof(Resource))]
        [Range(0, int.MaxValue, ErrorMessageResourceName = "OnlyPositive", ErrorMessageResourceType = typeof(Resource))]
        public long? Expense { get; set; } = 0;
        [Display(Name = "Profit", ResourceType = typeof(Resource))]
        public long? Profit { get; set; } = 0;
        [Display(Name = "Date", ResourceType = typeof(Resource))]
        public DateTime Date { get; set; } = DateTime.Now;

        public List<MonthlyExpense_task_expense>? Monthly_expenses_tasks_expenses { get; set; }
    }
    public class MonthlyExpense_task_expense
    {
        [Required]
        public int Id { get; set; }
        [ForeignKey("Monthly_expenses")]
        public int Monthly_expense_id { get; set; }
        public MonthlyExpense? Monthly_expenses { get; set; }
        public int? Expense_id { get; set; }
        public int? Task_id { get; set; }
    }
}

