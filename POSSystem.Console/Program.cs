using POSSystem.Application.Services;
using POSSystem.Infrastructure.Repositories;

ProductRepository repository = new ProductRepository();
ProductService service = new ProductService(repository);

Console.WriteLine("商品一覧");

foreach (var product in service.GetAllProducts())
{
    Console.WriteLine($"コード: {product.Code}, 名前: {product.Name}, 価格: {product.Price}, 在庫: {product.Stock}");
}

Console.WriteLine();

Console.Write("商品コードを入力してください: ");
string? code = Console.ReadLine();

var productInfo = service.GetProductByCode(code!);

if (productInfo == null)
{
    Console.WriteLine("商品が見つかりませんでした。");
}
else
{
    Console.WriteLine();
    Console.WriteLine("商品情報");
    Console.WriteLine($"商品名: {productInfo.Name}");
    Console.WriteLine($"価格: {productInfo.Price}");
    Console.WriteLine($"在庫: {productInfo.Stock}");
}



