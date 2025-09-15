using System.ComponentModel.DataAnnotations;

namespace EasyGames.ViewModels
{
    // This View Model is used when a user wants to change their password
    public class ChangePasswordViewModel
    {        
        [Required(ErrorMessage = "Email is required.")] // Email is required
        [EmailAddress] // Must be a valid email format
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")] // Password is required
        [StringLength(40, MinimumLength = 8, ErrorMessage = "The {0} must be at {2} and at max {1} characters long.")] // Password length constraints
        [DataType(DataType.Password)] // Input type is password
        [Display(Name = "New Password")] // Display name for the field
        [Compare("ConfirmNewPassword", ErrorMessage = "Password does not match.")] // Must match ConfirmNewPassword
        public string NewPassword { get; set; } = string.Empty; 

        [Required(ErrorMessage = "Confirm Password is required.")] // Confirm Password is required
        [DataType(DataType.Password)] // Input type is password
        [Display(Name = "Confirm New Password")] // Display name for the field
        public string ConfirmNewPassword { get; set; } = string.Empty;

    }
}