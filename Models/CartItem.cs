namespace EasyGames.Models
{
    // CartItem model denotes an item in the shopping cart
    public class CartItem
    {
        // Properties of Cart Item
        public int Id { get; set; } // StockItem Id
        public string Title { get; set; } = ""; // StockItem Title
        public decimal Price { get; set; } // StockItem Price
        public int Qty { get; set; } // Quantity added to cart
        public int MaxAvailable { get; set; } // How many are left in stock
    }
}
