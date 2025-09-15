namespace EasyGames.Models
{
    // HomeViewModel to pass data to the home page view
    public class HomeViewModel
    {
        // List of product categories
        public List<Category> Categories { get; set; } = new(); // All product categories
        public List<StockItem> WhatsNew { get; set; } = new(); // Newly added stock items
        public List<StockItem> Trending { get; set; } = new(); // Trending stock items   
        public bool IsAuthenticated { get; set; } // Is the user authenticated
        public bool IsAdmin { get; set; } // Is the user an admin
    }
}
