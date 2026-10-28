using System;
using System.Collections.Generic;
using System.Text;

namespace POSSystem.Domain.Entities
{
    public class CartItem
    {
        public int Id { get; set; }
        public int CartId { get; set; }
        public string ProductCode { get; set; } = string.Empty;
        public int Quantity { get; set; }
    }
}
