using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseTracker.Models.Entities
{
    public class Budget
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public Category? Category { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Budget amount must be greater than 0")]
        public decimal Amount { get; set; }

        [Required]
        public int Month { get; set; }

        [Required]
        public int Year { get; set; }

        public string? UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.Now;
    }
}
