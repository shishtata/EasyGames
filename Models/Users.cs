using Microsoft.AspNetCore.Identity;

namespace EasyGames.Models
{
    // Users class extends IdentityUser to include additional properties
    public class Users: IdentityUser

    {
        public string FullName { get; set; } = string.Empty; // Full name of the user
    }
}
