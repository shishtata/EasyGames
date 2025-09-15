using System.ComponentModel.DataAnnotations;

namespace EasyGames.Models
{
    // StockItem model represents an item in stock
    public class StockItem
    {
        // Properties of Stock Item
        public int Id { get; set; } // Primary key

        [Required, StringLength(180)] // Title is required and has a max length of 180 characters
        public string Title { get; set; } = default!;

        [StringLength(500)] // Description has a max length of 500 characters   
        public string? Description { get; set; }

        [Range(0, 100000)] // Price must be between 0 and 100000
        public decimal Price { get; set; }

        [Range(0, 100000)]  // Quantity must be between 0 and 100000
        public int Quantity { get; set; }

        // Optional URL to an image of the stock item
        public string? ImageUrl { get; set; }

        // Foreign key to the Category
        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        // UTC timestamp when the item was created
        public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
    }
}
