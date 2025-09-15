using EasyGames.Models;
using EasyGames.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EasyGames.Controllers
{
    // AccountController manages user authentication and registration
    public class AccountController : Controller
    {
        private readonly SignInManager<Users> signInManager; // Sign-in manager for handling user sign-in
        private readonly UserManager<Users> userManager; // User manager for handling user-related operations
        private readonly RoleManager<IdentityRole> roleManager; // Role manager for handling roles

        public AccountController(SignInManager<Users> signInManager, UserManager<Users> userManager, RoleManager<IdentityRole> roleManager)
        {
            // Constructor to initialize the AccountController with necessary services
            this.signInManager = signInManager; // Initialize sign-in manager
            this.userManager = userManager; // Initialize user manager
            this.roleManager = roleManager; // Initialize role manager
        }

        [AllowAnonymous] // Allow access to Login without authentication
        [HttpGet]
        public IActionResult Login()
        {
            return View(); // Return the Login view
        }

        [AllowAnonymous]  // Allow access to Login without authentication
        [HttpPost]
        [ValidateAntiForgeryToken] // Prevent CSRF attacks
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Attempt to sign in the user
            var result = await signInManager.PasswordSignInAsync(
                model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

            // If sign-in is successful, redirect to home page
            if (result.Succeeded)
                return RedirectToAction("Index", "Home");

            // If sign-in fails, display an error message
            ModelState.AddModelError(string.Empty, "Invalid Login Attempt.");
            return View(model);
        }

        // Registration actions
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Register()
        {
            return View(); // Return the Register view
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model) // Register new users
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = new Users
            {
                FullName = model.Name,
                UserName = model.Email,
                NormalizedUserName = model.Email.ToUpper(),
                Email = model.Email,
                NormalizedEmail = model.Email.ToUpper()
            };

            // Create the user with the specified password
            var result = await userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                // ensure "User" role exists, then assign
                if (!await roleManager.RoleExistsAsync("User"))
                    await roleManager.CreateAsync(new IdentityRole("User"));

                await userManager.AddToRoleAsync(user, "User");

                // DO NOT sign in here — ask them to log in
                TempData["Msg"] = "Registration successful. Please log in.";
                return RedirectToAction("Login", "Account");
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            return View(model);
        }

        [HttpGet] // Step 1: Verify Email
        public IActionResult VerifyEmail()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken] // Step 2: Handle Email Verification
        public async Task<IActionResult> VerifyEmail(VerifyEmailViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await userManager.FindByNameAsync(model.Email);

            if (user == null)
            {
                ModelState.AddModelError("", "User not found!");
                return View(model);
            }
            else
            {
                return RedirectToAction("ChangePassword", "Account", new { username = user.UserName });
            }
        }

        [HttpGet]
        public IActionResult ChangePassword(string username) // Step 3: Change Password
        {
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("VerifyEmail", "Account");
            }

            return View(new ChangePasswordViewModel { Email = username });
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model) // Step 4: Handle Password Change
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Something went wrong");
                return View(model);
            }

            // Find the user by email
            var user = await userManager.FindByNameAsync(model.Email);

            // If user not found, return error
            if (user == null)
            {
                ModelState.AddModelError("", "User not found!");
                return View(model);
            }

            // Remove the existing password and add the new password
            var result = await userManager.RemovePasswordAsync(user);
            if (result.Succeeded)
            {
                result = await userManager.AddPasswordAsync(user, model.NewPassword);
                return RedirectToAction("Login", "Account");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken] // Logout action to sign out the user
        public async Task<IActionResult> Logout() 
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

    }
}
