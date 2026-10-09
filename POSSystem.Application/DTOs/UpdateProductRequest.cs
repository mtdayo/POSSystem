using System;
using System.Collections.Generic;
using System.Text;

namespace POSSystem.Application.DTOs
{
    public class UpdateProductRequest
    {
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Stock { get; set; }
    }
}
