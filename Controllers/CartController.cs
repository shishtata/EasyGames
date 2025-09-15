using EasyGames.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyGames.Controllers
{
    // CartController manages the shopping cart functionality
    public class CartController(CartService cart) : Controller
    {
        private readonly CartService _cart = cart; // CartService for managing cart operations

        [Authorize] // Ensure user is authenticated to access cart
        public IActionResult Index()
        {
            var items = _cart.All();
            ViewBag.Total = _cart.Total();
            return View(items);
        }

        [Authorize] // Ensure user is authenticated to add items to cart
        [HttpPost]
        public async Task<IActionResult> Add(int id, int qty = 1)
        {
            await _cart.AddAsync(id, qty);
            return RedirectToAction("Index", "Catalog");
        }

        [HttpPost]
        // Update item quantity in cart
        public IActionResult Update(int id, int qty)
        {
            _cart.Update(id, qty);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        // Remove item from cart
        public IActionResult Remove(int id)
        {
            _cart.Remove(id);
            return RedirectToAction(nameof(Index));
        }

        [Authorize]
        [HttpPost]
        // Handle checkout process
        public async Task<IActionResult> Checkout()
        {
            try
            {
                if (!await _cart.CanCheckoutAsync())
                {
                    TempData["Msg"] = "One or more items exceed available stock.";
                    return RedirectToAction(nameof(Index));
                }

                await _cart.CheckoutAsync();
                TempData["Msg"] = "Order placed successfully.";
                return RedirectToAction("Index", "Catalog");
            }
            catch (Exception ex)
            {
                TempData["Msg"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
