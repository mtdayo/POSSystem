using POSSystem.Application.Services;
using POSSystem.Domain.Entities;
using POSSystem.Infrastructure.Repositories;

ProductRepository repository = new ProductRepository();
ProductService service = new ProductService(repository);
Order order = new Order();

int quantity;
decimal payment;

Console.WriteLine("商品一覧");

foreach (var product in service.GetAllProducts())
{
    Console.WriteLine($"コード: {product.Code}, 名前: {product.Name}, 価格: {product.Price}, 在庫: {product.Stock}");
}



while (true)
{
    Console.WriteLine();

    Console.Write("追加したい商品コードを入力してください(exitで終了): ");
    string? code = Console.ReadLine();

    if (code!.Equals("exit", StringComparison.OrdinalIgnoreCase))
    {
        break;
    }

    var productInfo = service.GetProductByCode(code!);


    if (productInfo == null)
    {
        Console.WriteLine("商品が見つかりませんでした。");
        continue;
    }

    Console.Write($"{productInfo.Name}の数量を入力してください: ");
    if (!int.TryParse(Console.ReadLine(), out quantity))
    {
        Console.WriteLine("数量は整数値で入力してください。");
        continue;
    }

    if (quantity <= 0)
    {
        Console.WriteLine("無効な数量です。");
        continue;
    }
    else if (quantity > productInfo.Stock)
    {
        Console.WriteLine($"在庫が不足しています。現在の在庫: {productInfo.Stock}");
        continue;
    }

    order.AddItem(productInfo, quantity);
    productInfo.ReduceStock(quantity);
    Console.WriteLine($"{productInfo.Name}を{quantity}個追加しました。");

}

while (true)
{
    Console.WriteLine();

    Console.Write("削除したい商品コードを入力してください(exitで終了): ");
    string? code = Console.ReadLine();

    if (code!.Equals("exit", StringComparison.OrdinalIgnoreCase))
    {
        break;
    }

    var productInfo = service.GetProductByCode(code!);
    if (productInfo == null)
    {
        Console.WriteLine("商品が見つかりませんでした。");
        continue;
    }

    Console.Write($"{productInfo.Name}の数量を入力してください: ");
    if (!int.TryParse(Console.ReadLine(), out quantity))
    {
        Console.WriteLine("数量は整数値で入力してください。");
        continue;
    }


    try
    {
        var removeItem = order.RemoveItem(code, quantity);

        if (removeItem == null)
        {
            Console.WriteLine("注文にその商品は存在しません。");
            continue;
        }

        removeItem.Product.AddStock(quantity);
        Console.WriteLine($"{removeItem.Product.Name}を{quantity}個削除しました。");
    }
    catch (ArgumentOutOfRangeException ex)
    {
        Console.WriteLine($"エラー: {ex.Message}");
    }
    catch (InvalidOperationException ex)
    {
        Console.WriteLine($"エラー: {ex.Message}");
    }
}

decimal receivedAmount = 0;

while (true)
{

    Console.WriteLine();
    Console.Write($"合計金額は{order.Total}円です。支払金額を入力してください: ");

    if (!decimal.TryParse(Console.ReadLine(), out payment))
    {
        Console.WriteLine("金額は数値で入力してください。");
        continue;
    }

    receivedAmount += payment;

    decimal shortage = order.Total - receivedAmount;

    if (shortage > 0)
    {
        Console.WriteLine($"不足金額: {shortage}円、現在のお預かり金額: {receivedAmount}");
        continue;
    }

    decimal change = receivedAmount - order.Total;
    Console.WriteLine($"お預かり金額: {receivedAmount}円");
    Console.WriteLine($"お釣り: {change}円");
    break;
}

Console.WriteLine();
Console.WriteLine("注文内容:");
foreach (var item in order.Items)
{
    Console.WriteLine($"商品: {item.Product.Name}, 数量: {item.Quantity}, 小計: {item.Subtotal}円");
}
Console.WriteLine($"合計: {order.Total}円");
