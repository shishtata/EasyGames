namespace EasyGames.Models
{
    // Category model represents a product category
    public class Category
    {
        // Properties of Category
        public int Id { get; set; } // Primary key
        public string Name { get; set; } = default!; // Category name
        public List<StockItem> Items { get; set; } = new(); // Navigation property to related StockItems
    }
}
