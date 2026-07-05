using System;
using System.Collections.Generic;
using System.Text;

namespace POSSystem.Domain.Entities
{
    public class Product
    {
        public int Id { get; }
        public String Code { get; }  //商品コード
        public string Name { get; }
        public decimal Price { get; }
        public int Stock { get; private set; }

        public Product(int id, String code, string name, decimal price, int stock)
        {
            Id = id;
            Code = code;
            Name = name;
            Price = price;
            Stock = stock;
        }

        public void ReduceStock(int quantity)
        {
            if (quantity <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(quantity), "数量は正の整数でなければなりません。");
            }

            if (quantity > Stock)
            {
                throw new InvalidOperationException($"在庫が不足しています。現在の在庫: {Stock}");
            }

            Stock -= quantity;
        }

        public void AddStock(int quantity)
        {
            if (quantity <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(quantity), "数量は正の整数でなければなりません。");
            }
            Stock += quantity;
        } 
    }
}
