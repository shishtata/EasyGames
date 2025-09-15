using System.ComponentModel.DataAnnotations;

namespace EasyGames.ViewModels
{
    // This View Model is used for user registration
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Name is required.")] // Name is required
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")] // Email is required
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")] // Password is required
        [StringLength(40, MinimumLength = 8, ErrorMessage = "The {0} must be at {2} and at max {1} characters long.")] // Password length constraints
        [DataType(DataType.Password)] // Input type is password
        [Compare("ConfirmPassword", ErrorMessage = "Password does not match.")] // Must match ConfirmPassword
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirm Password is required.")] // Confirm Password is required
        [DataType(DataType.Password)] // Input type is password
        [Display(Name = "Confirm Password")] // Display name for the field
        public string ConfirmPassword { get; set; } = string.Empty;

    }
}