using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace POSSystem.Domain.Entities
{
    public class Order
    {
        private readonly List<OrderItem> _items = new List<OrderItem>();
        public IReadOnlyList<OrderItem> Items => _items;
        public decimal Total => _items.Sum(item => item.Subtotal);
        public void AddItem(Product product, int quantity)
        {
            var item = _items.FirstOrDefault(i => i.Product.Code == product.Code);
            if (item == null)
            {
                _items.Add(new OrderItem(product, quantity));
            }
            else
            {
                item.AddQuantity(quantity);
            }
        }

        public OrderItem? RemoveItem(string productCode, int quantity)
        {
            var item = _items.FirstOrDefault(i => i.Product.Code == productCode);

            if (item == null) return null;


            item.ReduceQuantity(quantity);

            if (quantity == 0)
            {
                _items.Remove(item);
            }

            return item;
        }
    }
}
