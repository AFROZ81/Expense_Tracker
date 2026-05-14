using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Models.Entities
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [StringLength(100)]
        public string? FullName { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.Now;
    }
}
