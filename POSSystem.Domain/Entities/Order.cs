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
            _items.Add(new OrderItem(product, quantity));
        }
    }
}
