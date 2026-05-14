using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseTracker.Models.Entities
{
    public enum AccountType
    {
        Cash,
        Bank,
        [Display(Name = "Credit Card")]
        CreditCard,
        Savings,
        Investment,
        Other
    }

    public class Account
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string? Name { get; set; }

        [Required]
        [Display(Name = "Account Type")]
        public AccountType Type { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Initial Balance")]
        public decimal InitialBalance { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Current Balance")]
        public decimal CurrentBalance { get; set; }

        public bool IsActive { get; set; } = true;

        public string? UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.Now;

        // Navigation properties
        public virtual ICollection<Expense>? Expenses { get; set; }
        public virtual ICollection<Income>? Incomes { get; set; }
    }
}
