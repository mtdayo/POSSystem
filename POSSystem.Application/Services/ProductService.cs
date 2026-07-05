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

        public void ShowAllProduct(ProductService service)
        {
            Console.WriteLine();

            Console.WriteLine("商品一覧");

            foreach (var product in service.GetAllProducts())
            {
                Console.WriteLine($"コード: {product.Code}, 名前: {product.Name}, 価格: {product.Price}, 在庫: {product.Stock}");
            }
        }
    }
}
