using EasyGames.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace EasyGames.Services
{
    // Represents one item in the shopping cart
    public class CartItem
    {
        public int StockItemId { get; set; } // ID of the product
        public string Title { get; set; } = default!; // Name of the product
        public int Quantity { get; set; } // How many of this product
        public decimal UnitPrice { get; set; } // Price per unit
        public decimal LineTotal => UnitPrice * Quantity; // Total price for this line
        public int MaxAvailable { get; set; } // Max available stock
    }

    // CartService manages the shopping cart using session storage
    public class CartService
    {
        private const string Key = "CART_V1"; // Session key for the cart
        private readonly AppDbContext _db; // Database context
        private readonly IHttpContextAccessor _http; // HTTP context accessor

        private ISession Session => _http.HttpContext!.Session; // Current session

        // Constructor injects database + HTTP context
        public CartService(AppDbContext db, IHttpContextAccessor http) 
        {
            _db = db;
            _http = http;
        }

        // Reads cart from session or returns empty list
        private List<CartItem> GetCart()
        {
            var s = Session.GetString(Key);
            return s == null ? new List<CartItem>() : JsonSerializer.Deserialize<List<CartItem>>(s)!;
        }

        // Saves cart to session
        private void SaveCart(List<CartItem> cart) =>
            Session.SetString(Key, JsonSerializer.Serialize(cart));

        // Adds item to cart or updates quantity
        public async Task AddAsync(int stockId, int qty = 1)
        {
            var stock = await _db.StockItems.FindAsync(stockId) 
                        ?? throw new Exception("Item not found."); // Ensure item exists


            var cart = GetCart();
            var line = cart.FirstOrDefault(x => x.StockItemId == stockId);

            // Calculate desired quantity (current + added), at least 1
            var desired = (line?.Quantity ?? 0) + Math.Max(1, qty);

            // Limits quantity to available stock only
            desired = Math.Min(desired, stock.Quantity);

            if (line == null)
            {
                // New item, only add if desired > 0, which means if item is not in cart, add it
                if (desired > 0)
                    cart.Add(new CartItem
                    {
                        StockItemId = stock.Id,
                        Title = stock.Title,
                        UnitPrice = stock.Price,
                        Quantity = desired,
                        MaxAvailable = stock.Quantity
                    });
            }
            else
            {
                // If the item already exists, update the quantity and max available
                line.Quantity = Math.Max(1, desired);
                line.MaxAvailable = stock.Quantity;
            }

            SaveCart(cart);
        }

        // Get all items currently in the cart
        public List<CartItem> All() => GetCart();

        // Update quantity of a specific item in the cart manually
        public void Update(int id, int qty)
        {
            var cart = GetCart();
            var line = cart.FirstOrDefault(x => x.StockItemId == id);
            if (line != null)
            {
                var stock = _db.StockItems.Find(id);
                if (stock != null)
                {
                    // Limit/Constraint: at least 1, at most available stock
                    line.Quantity = Math.Min(Math.Max(1, qty), stock.Quantity);
                    line.MaxAvailable = stock.Quantity;
                }
            }
            SaveCart(cart);
        }

        // Remove item from cart by ID
        public void Remove(int id) =>
            SaveCart(GetCart().Where(x => x.StockItemId != id).ToList());

        // Calculate total cost of all items in the cart
        public decimal Total() => GetCart().Sum(x => x.LineTotal);

        // Check if all items in the cart can be checked out (enough stock)
        public async Task<bool> CanCheckoutAsync()
        {
            var cart = GetCart();
            var ids = cart.Select(c => c.StockItemId).ToList();
            var stocks = await _db.StockItems.Where(s => ids.Contains(s.Id)).ToListAsync();

            return cart.All(ci =>
                stocks.First(s => s.Id == ci.StockItemId).Quantity >= ci.Quantity);
        }

        // Perform checkout: validate stock, decrement quantities, clear cart
        public async Task CheckoutAsync()
        {
            var cart = GetCart();
            if (cart.Count == 0) return;

            var ids = cart.Select(c => c.StockItemId).ToList();
            var stocks = await _db.StockItems.Where(s => ids.Contains(s.Id)).ToListAsync();

            // Validate stock availability
            foreach (var ci in cart)
            {
                var s = stocks.First(x => x.Id == ci.StockItemId);
                if (s.Quantity < ci.Quantity)
                    throw new InvalidOperationException($"Not enough stock for {s.Title}");
            }

            // Decrement stock quantities i.e. remove those checked out items from stock
            foreach (var ci in cart)
            {
                var s = stocks.First(x => x.Id == ci.StockItemId);
                s.Quantity -= ci.Quantity;
            }

            await _db.SaveChangesAsync();
            Clear();
        }

        // Clear the cart completely
        public void Clear() => SaveCart(new());
    }
}
