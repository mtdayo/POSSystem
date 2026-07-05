using POSSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace POSSystem.Infrastructure.Repositories
{
    public class SaleRepository
    {
        private readonly List<Sale> _sales = new List<Sale>();

        public void AddSale(Sale sale)
        {
            _sales.Add(sale);
        }

        public IReadOnlyList<Sale> GetAllSales()
        {
            return _sales;
        }
    }
}
