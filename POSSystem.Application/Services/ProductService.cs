using POSSystem.Domain.Entities;
using POSSystem.Infrastructure.Repositories;

namespace POSSystem.Application.Services
{
    public class ProductService
    {
        private readonly ProductRepository _repository;

        public ProductService(ProductRepository repository)
        {
            _repository = repository;
        }

        public IReadOnlyList<Product> GetAllProducts()
        {
            return _repository.GetAllProducts();
        }

        public Product? GetProductByCode(string code)
        {
            return _repository.GetProductByCode(code);
        }
    }
}
