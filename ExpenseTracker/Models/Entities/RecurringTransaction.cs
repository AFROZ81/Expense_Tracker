using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseTracker.Models.Entities
{
    public enum Frequency
    {
        Daily,
        Weekly,
        Monthly,
        Yearly
    }

    public enum TransactionType
    {
        Expense,
        Income
    }

    public class RecurringTransaction
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string? Description { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required]
        public Frequency Frequency { get; set; }

        [Required]
        public TransactionType Type { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        public DateTime? LastProcessedDate { get; set; }

        public DateTime? EndDate { get; set; }

        public bool IsActive { get; set; } = true;

        [Required]
        public int CategoryId { get; set; } // Can be Expense Category or Income Category depending on Type

        [Required]
        public int AccountId { get; set; }
        public virtual Account? Account { get; set; }

        public string? UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }
    }
}
