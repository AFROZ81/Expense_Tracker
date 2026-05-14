using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseTracker.Models.Entities
{
    public class Income
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Income Source")]
        public int IncomeCategoryId { get; set; }

        [ForeignKey("IncomeCategoryId")]
        public IncomeCategory? IncomeCategory { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Amount { get; set; }

        [Required]
        [Display(Name = "Income Date")]
        public DateTime IncomeDate { get; set; }

        [StringLength(250)]
        public string? Description { get; set; }

        [Display(Name = "Deposit Account")]
        public int? AccountId { get; set; }

        [ForeignKey("AccountId")]
        public Account? Account { get; set; }

        public string? UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.Now;
    }
}
