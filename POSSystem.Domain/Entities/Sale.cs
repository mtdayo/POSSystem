using System;
using System.Collections.Generic;
using System.Text;

namespace POSSystem.Domain.Entities
{
    public class Sale
    {
        public int Id { get; }
        public DateTime SaleDate { get; }
        public IReadOnlyList<OrderItem> Items { get; }
        public decimal TotalAmount { get; }

        public Sale(int id, DateTime saleDate, IReadOnlyList<OrderItem> items, decimal totalAmount)
        {
            Id = id;
            SaleDate = saleDate;
            Items = items;
            TotalAmount = totalAmount;
        }
    }
}
