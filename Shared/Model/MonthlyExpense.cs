using Cargotruck.Shared.Resources;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cargotruck.Shared.Model
{
    public class MonthlyExpense
    {
        [Required]
        public int Id { get; set; }
        public string? UserId { get; set; }
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
        public int MonthlyExpenseId { get; set; }
        public MonthlyExpense? MonthlyExpense { get; set; }
        public int? ExpenseId { get; set; }
        public int? TaskId { get; set; }
    }
}

