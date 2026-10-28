namespace POSSystem.WebApp.Models
{
    public class CartItem
    {
        public int CartItemId { get; set; }
        public Product Product { get; set; } = default!;
        public int Quantity {  get; set; }
        public decimal Subtotal => Product.Price * Quantity;
    }
}
