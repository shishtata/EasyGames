using System.ComponentModel.DataAnnotations;

namespace EasyGames.ViewModels
{
    // This View Model is used when a user wants to verify their email
    public class VerifyEmailViewModel
    {
        [Required(ErrorMessage = "Email is required.")] // Email is required
        [EmailAddress] // Must be a valid email format
        public string Email { get; set; } = string.Empty;

    }
}