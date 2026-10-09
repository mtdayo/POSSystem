using System;
using System.Collections.Generic;
using System.Text;

namespace POSSystem.Application.DTOs
{
    public class AddProductRequest
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Stock { get; set; }
    }
}
