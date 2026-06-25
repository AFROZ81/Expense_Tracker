using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Models.ViewModels
{
    public class EditProfileViewModel
    {
        [Required(ErrorMessage = "Full Name is required.")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email Address is required.")]
        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        [Display(Name = "Email Address")]
        public string Email { get; set; } = string.Empty;

    }
}
