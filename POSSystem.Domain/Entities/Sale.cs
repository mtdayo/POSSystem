using POSSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace POSSystem.Domain.Entities
{
    public class Sale
    {
        public int Id { get; private set; }
        public int AccountId { get; private set; }
        public DateTime SaleDate { get; private set; }
        public IReadOnlyList<SaleItem> Items { get; private set; }
        public decimal TotalAmount { get; private set; }
        public Payment Payment { get; private set; }

        public Sale(int id, int accountId, DateTime saleDate, IReadOnlyList<SaleItem> items, decimal totalAmount, Payment payment)
        {
            Id = id;
            AccountId = accountId;
            SaleDate = saleDate;
            Items = items;
            TotalAmount = totalAmount;
            Payment = payment;
        }
    }
}
