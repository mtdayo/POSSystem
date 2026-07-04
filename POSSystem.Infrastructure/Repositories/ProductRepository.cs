using POSSystem.Domain.Entities;

namespace POSSystem.Infrastructure.Repositories
{
    public class ProductRepository
    {
        private readonly List<Product> _products = new()
        {
            new Product(1, "P001", "りんご", 100m, 100),
            new Product(2, "P002", "みかん", 150m, 50),
            new Product(3, "P003", "ばなな", 200m, 25)
        };

        public IReadOnlyList<Product> GetAllProducts()
        {
            return _products;
        }

        public Product? GetProductByCode(string code)
        {
            return _products.FirstOrDefault(p => p.Code == code);
        }
    }
}
