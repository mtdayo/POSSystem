using System;
using System.Collections.Generic;
using System.Text;

namespace POSSystem.Application.DTOs
{
    public class CheckoutRequest
    {
        public List<CheckoutItemRequest> Items { get; set; } = new();

        public decimal RecievedAmount { get; set; }
    }
}
