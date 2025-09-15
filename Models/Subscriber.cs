using System.ComponentModel.DataAnnotations;

namespace EasyGames.Models
{
    // Subscribers model represents a subscriber with an email address
    public class Subscribers
    {
        [Required]
        public int Id { get; set; } // Primary key

        [Required, EmailAddress, MaxLength(256)] // Email is required, must be a valid email format, and has a max length of 256 characters
        public string Email { get; set; } = string.Empty; // Subscriber's email address

    }
}
