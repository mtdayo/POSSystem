namespace POSSystem.WebApp.Models
{
    public class CheckoutItemRequest
    {
        public string Code { get; set;  } = string.Empty;
        public int Quantity { get; set; }
    }
}
