using System;
using System.Collections.Generic;
using System.Text;

namespace POSSystem.Application.DTOs
{
    public class ProductSearchRequest
    {
        public string? Search { get; set; }
        public string SortBy { get; set; } = "code";
        public string SortOrder { get; set; } = "asc";
    }
}
