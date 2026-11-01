namespace POSSystem.WebApp.Models
{
    public class ProductSalesSummary
    {
        public string ProductName { get; set; } = "";
        public string ProductCode { get; set; } = "";
        public int TotalQuantity { get; set; }
        public decimal TotalSales { get; set; }
    }
}
