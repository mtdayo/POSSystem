using System;
using System.Collections.Generic;
using System.Text;

namespace POSSystem.Application.DTOs
{
    public class CheckoutItemRequest
    {
        public string Code { get; set; } = string.Empty;

        public int Quantity { get; set; }
    }
}
