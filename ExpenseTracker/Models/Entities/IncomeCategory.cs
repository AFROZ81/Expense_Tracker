using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Models.Entities
{
    public class IncomeCategory
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string? Name { get; set; }

        public bool IsActive { get; set; } = true;

        public string? UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }
    }
}
