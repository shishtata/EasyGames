using EasyGames.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EasyGames.Controllers
{
    // UsersController manages user-related actions, restricted to Admin role
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly UserManager<Users> _userManager;
        public UsersController(UserManager<Users> userManager) => _userManager = userManager;

        // List all users
        public IActionResult Index() => View(_userManager.Users.ToList());

        // Confirm delete page
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id); // Find user by ID
            if (user == null) return NotFound(); // Return 404 if user not found
            return View(user); // Pass user to the view for confirmation
        }

        // Actually delete the user
        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken] // POST method for deletion with anti-forgery token
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _userManager.FindByIdAsync(id); // Find user by ID
            if (user != null) await _userManager.DeleteAsync(user); // Delete user if found
            return RedirectToAction(nameof(Index)); // Redirect to user list after deletion
        }
    }
}
