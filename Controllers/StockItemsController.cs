using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EasyGames.Data;
using EasyGames.Models;
using Microsoft.AspNetCore.Authorization;

namespace EasyGames.Controllers
{
    // StockItemsController manages CRUD operations for stock items
    [Authorize(Roles = "Admin")]
    public class StockItemsController : Controller
    {
        private readonly AppDbContext _context;

        public StockItemsController(AppDbContext context)
        {
            _context = context;
        }
                
        public async Task<IActionResult> Index()
        {
            // List all stock items with their categories
            var appDbContext = _context.StockItems.Include(s => s.Category);
            return View(await appDbContext.ToListAsync());
        }
                
        public async Task<IActionResult> Details(int? id)
        {
            // View details of a specific stock item
            if (id == null)
            {
                return NotFound();
            }

            var stockItem = await _context.StockItems
                .Include(s => s.Category)
                .FirstOrDefaultAsync(m => m.Id == id); // Find the stock item by ID
            if (stockItem == null)
            {
                return NotFound();
            }

            return View(stockItem);
        }

        public IActionResult Create()
        {
            // Load the create view with category data
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            return View();
        }

        
        [ValidateAntiForgeryToken] // Prevent CSRF attacks
        // Create a new stock item
        public async Task<IActionResult> Create([Bind("Id,Title,Description,Price,Quantity,ImageUrl,CategoryId")] StockItem stockItem)
        {
            if (ModelState.IsValid)
            {
                // Add new stock item
                _context.Add(stockItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // If model state is invalid, reload the create view with category data
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", stockItem.CategoryId);
            return View(stockItem);
        }

       
        public async Task<IActionResult> Edit(int? id)
        {
            // Edit a stock item
            if (id == null)
            {
                return NotFound();
            }

            // Find the stock item to edit
            var stockItem = await _context.StockItems.FindAsync(id);
            if (stockItem == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", stockItem.CategoryId); // Load categories for dropdown
            return View(stockItem);
        }
               
        [HttpPost]
        [ValidateAntiForgeryToken]

        // Edit a stock item
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Price,Quantity,ImageUrl,CategoryId")] StockItem stockItem)
        {
            if (id != stockItem.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // Update the stock item
                try
                {
                    _context.Update(stockItem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    //  Handle concurrency issues during update
                    if (!StockItemExists(stockItem.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", stockItem.CategoryId); // Reload categories if model state is invalid
            return View(stockItem);
        }

        
        public async Task<IActionResult> Delete(int? id)
        {
            // Confirm deletion of a stock item
            if (id == null)
            {
                return NotFound();
            }

            // Find the stock item to delete
            var stockItem = await _context.StockItems
                .Include(s => s.Category)
                .FirstOrDefaultAsync(m => m.Id == id); // Find the stock item by ID
            if (stockItem == null)
            {
                return NotFound();
            }

            return View(stockItem);
        }
               
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]

        // Confirm deletion of a stock item
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Find and remove the stock item
            var stockItem = await _context.StockItems.FindAsync(id);
            if (stockItem != null)
            {
                _context.StockItems.Remove(stockItem);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index)); // Redirect to Index after deletion
        }

        private bool StockItemExists(int id)
        {
            // 
            return _context.StockItems.Any(e => e.Id == id); // Check if a stock item exists by ID
        }
    }
}
