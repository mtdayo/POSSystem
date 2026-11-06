using System;
using System.Collections.Generic;
using System.Text;

namespace POSSystem.Domain.Entities
{
    public class OrderItem
    {
        public Product Product { get; }
        public int Quantity { get; private set; }
        public decimal Subtotal => Product.Price * Quantity;

        public OrderItem(Product product, int quantity)
        {
            Product = product;
            Quantity = quantity;
        }

        public void AddQuantity(int quantity)
        {
            Quantity += quantity;
        }

        public void ReduceQuantity(int quantity)
        {
            if (quantity <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(quantity), "数量は正の整数でなければなりません。");
            }

            if (quantity > Quantity)
            {
                throw new InvalidOperationException($"削除できる数量は最大{Quantity}個です。");
            }

            Quantity -= quantity;
        }
    }
}
