using EasyGames.Data;
using EasyGames.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EasyGames.Controllers
{
    // CatalogController handles displaying and filtering stock items
    public class CatalogController : Controller
    {
        // Database context is used here for accessing stock items and categories
        private readonly AppDbContext _db;

        // Constructor to initialize the CatalogController with the database context
        public CatalogController(AppDbContext db)
        {
            _db = db;
        }

        // Index action displays the catalog with optional filtering by category and search query
        public async Task<IActionResult> Index(int? categoryId, string? q)
        {
            // Get all categories for the dropdown
            var cats = await _db.Categories.OrderBy(c => c.Name).ToListAsync();
            ViewBag.Categories = cats;
            ViewBag.SelectedCategoryId = categoryId;

            // Start with all stock items
            var items = _db.StockItems.Include(s => s.Category).AsQueryable();

            // Filter by category
            if (categoryId.HasValue)
            {
                items = items.Where(s => s.CategoryId == categoryId.Value);
            }

            // Filter by search
            if (!string.IsNullOrWhiteSpace(q))
            {
                items = items.Where(s =>
                    s.Title.Contains(q) || (s.Description ?? "").Contains(q));
            }

            // Return the filtered list to the view
            return View(await items.ToListAsync());
        }
    }
}
