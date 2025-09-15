namespace EasyGames.Models
{
    // AdminDashboard model to hold statistics for the admin dashboard view
    public class AdminDashboard
    {
        // Total counts for various entities
        public int TotalUsers { get; set; } // Total number of registered users
        public int TotalCategories { get; set; } // Total number of product categories
        public int TotalStockItems { get; set; } // Total number of stock items
        public int TotalSubscribers { get; set; } // Total number of newsletter subscribers
        public List<StockItem> LowStock { get; set; } = new(); // List of stock items that are low in stock
        public int LowStockThreshold { get; set; } = 5; // Threshold for low stock items



    }
}
