using EasyGames.Data;
using EasyGames.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace EasyGames.Controllers
{
    // HomeController manages the home page and admin dashboard
    public class HomeController : Controller
    {
        private readonly AppDbContext _db; // Database context for accessing the database
        private readonly UserManager<Users> _userManager; // UserManager for managing user-related operations

        public HomeController(AppDbContext db, UserManager<Users> userManager)
        {
            // Constructor to initialize HomeController with database context and user manager
            _db = db;
            _userManager = userManager;
        }

        // Index action displays the home page with categories, new arrivals, and trending items
        public async Task<IActionResult> Index()
        {
            var vm = new HomeViewModel
            {                
                IsAuthenticated = User?.Identity?.IsAuthenticated == true,
                IsAdmin = User?.IsInRole("Admin") == true,
                Categories = await _db.Categories
                                        .AsNoTracking()
                                        .OrderBy(c => c.Name)
                                        .ToListAsync(), // Fetch categories
                WhatsNew = await _db.StockItems
                                     .Include(s => s.Category)
                                     .Where(s => s.Quantity > 0)
                                     .OrderByDescending(s => s.CreatedUtc) 
                                     .Take(8)
                                     .ToListAsync(), // Fetch new arrivals
                Trending = await _db.StockItems
                                     .Include(s => s.Category)
                                     .Where(s => s.Quantity > 0)
                                     .OrderBy(s => s.Quantity)       
                                     .Take(8)
                                     .ToListAsync() // Fetch trending items
            };

            return View(vm); // Return the view with the populated ViewModel
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]

        // Add a new subscriber from the admin dashboard
        public async Task<IActionResult> AddSubscriber(string email)
        {
            if (!string.IsNullOrWhiteSpace(email))
            {
                if (!await _db.Subscribers.AnyAsync(s => s.Email == email))
                {
                    // Add new subscriber if not already present
                    _db.Subscribers.Add(new Subscribers { Email = email });
                    await _db.SaveChangesAsync();
                    TempData["Msg"] = "Subscriber added.";
                }
                else
                {
                    // Inform if subscriber already exists
                    TempData["Err"] = "Subscriber already exists.";
                }
            }
            else
            {
                // Handle invalid email input
                TempData["Err"] = "Please enter a valid email.";
            }
            return RedirectToAction("Admin");
        }


        [Authorize(Roles = "Admin")]
        // Admin action displays the admin dashboard with various statistics
        public async Task<IActionResult> Admin()
        {
            const int threshold = 5; // Low stock threshold

            var vm = new AdminDashboard
            {
                // Fetch various statistics for the dashboard
                TotalUsers = await _userManager.Users.CountAsync(),
                TotalCategories = await _db.Categories.CountAsync(),
                TotalStockItems = await _db.StockItems.CountAsync(),
                TotalSubscribers = await _db.Subscribers.CountAsync(),
                LowStock = await _db.StockItems
                    .Include(s => s.Category)
                    .Where(s => s.Quantity <= threshold)
                    .OrderBy(s => s.Quantity).ThenBy(s => s.Title)
                    .ToListAsync(),
                LowStockThreshold = threshold
            };

            return View(vm);
        }

        [HttpPost]
        [HttpPost]
        // Subscribe action handles email subscriptions from the home page
        public async Task<IActionResult> Subscribe(string email)
        {
            email = email?.Trim().ToLowerInvariant(); // Normalize email

            if (string.IsNullOrWhiteSpace(email))
            {
                TempData["Err"] = "Please enter a valid email.";
                return RedirectToAction(nameof(Index));
            }

            // Simple format check
            var attr = new System.ComponentModel.DataAnnotations.EmailAddressAttribute();
            if (!attr.IsValid(email))
            {
                TempData["Err"] = "That doesn't look like an email.";
                return RedirectToAction(nameof(Index));
            }

            // Add subscriber if not already present
            if (!await _db.Subscribers.AnyAsync(s => s.Email == email))
            {
                _db.Subscribers.Add(new Subscribers { Email = email });
                await _db.SaveChangesAsync();
            }

            // Thank the user for subscribing message pops up
            TempData["Msg"] = "Thanks! You’re subscribed.";
            return RedirectToAction(nameof(Index));
        }


    }
}
