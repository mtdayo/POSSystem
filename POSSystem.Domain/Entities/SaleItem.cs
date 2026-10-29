using System;
using System.Collections.Generic;
using System.Text;

namespace POSSystem.Domain.Entities
{
    public class SaleItem
    {
        public int Id { get; private set; }
        public int SaleId { get; private set; }
        public string ProductCode { get; private set; }
        public string ProductName { get; private set; }
        public decimal Price { get; private set; }
        public int Quantity { get; private set; }
        public decimal SubTotal { get; private set; }

        public SaleItem (int id, int saleId, string productCode, string productName, decimal price, int quantity, decimal subTotal)
        {
            Id = id;
            SaleId = saleId;
            ProductCode = productCode;
            ProductName = productName;
            Price = price;
            Quantity = quantity;
            SubTotal = subTotal;
        }
    }
}
