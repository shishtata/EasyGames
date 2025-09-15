using System.ComponentModel.DataAnnotations;

namespace EasyGames.ViewModels
{
    // This View Model is used for user login
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email is required.")] // Email is required
        [EmailAddress] // Must be a valid email format
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")] // Password is required
        [DataType(DataType.Password)] // Input type is password
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Remember me?")] // Display name for the field
        public bool RememberMe { get; set; }

    }
}