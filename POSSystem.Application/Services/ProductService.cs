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

        public void AddProduct (Product product)
        {
            var existingProduct = _repository.GetProductByCode(product.Code);

            if (existingProduct != null)
            {
                throw new InvalidOperationException($"商品コード '{product.Code}' は既に存在します。");
            }


            _repository.AddProduct(product);
        }

        public bool UpdateProduct(Product product)
        {
            return _repository.UpdateProduct(product);
        }

        public bool DeleteProduct(string code)
        {
            return _repository.DeleteProduct(code);
        }

        public List<Product> SearchProducts(string? search, string sortBy, string sortOrder)
        {
            return _repository.SearchProducts(search, sortBy, sortOrder);
        }
    }
}
