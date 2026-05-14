using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseTracker.Models.Entities
{
    public class Expense
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public Category? Category { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Amount { get; set; }

        [Required]
        [Display(Name = "Expense Date")]
        public DateTime ExpenseDate { get; set; }

        [StringLength(250)]
        public string? Description { get; set; }

        [Display(Name = "Payment Account")]
        public int? AccountId { get; set; }

        [ForeignKey("AccountId")]
        public Account? Account { get; set; }

        public string? UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.Now;
    }
}
