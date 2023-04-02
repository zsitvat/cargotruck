using Cargotruck.Shared.Resources;
using System.ComponentModel.DataAnnotations;

namespace Cargotruck.Shared.Model.Dto
{
    public class MonthlyExpenseDto
    {
        [Required]
        public int Id { get; set; }
        public string? UserId { get; set; }
        [Display(Name = "Earning", ResourceType = typeof(Resource))]
        [Range(0, int.MaxValue, ErrorMessageResourceName = "OnlyPositive", ErrorMessageResourceType = typeof(Resource))]
        public int? Earning { get; set; } = 0;
        [Display(Name = "Expense", ResourceType = typeof(Resource))]
        [Range(0, int.MaxValue, ErrorMessageResourceName = "OnlyPositive", ErrorMessageResourceType = typeof(Resource))]
        public int? Expense { get; set; } = 0;
        [Display(Name = "Profit", ResourceType = typeof(Resource))]
        public int? Profit { get; set; } = 0;
        [Display(Name = "Date", ResourceType = typeof(Resource))]
        public DateTime Date { get; set; } = DateTime.Now;
    }
    public class MonthlyExpense_task_expenseDto
    {
        [Required]
        public int Id { get; set; }
        public int MonthlyExpenseId { get; set; }
        public MonthlyExpenseDto? MonthlyExpense { get; set; }
        public int? ExpenseId { get; set; }
        public int? TaskId { get; set; }
    }
}

