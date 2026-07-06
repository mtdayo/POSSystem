using POSSystem.Domain.Entities;
using POSSystem.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace POSSystem.Application.Services
{
    public class OrderService
    {

        public void AddProduct(ProductService service, Order order)
        {
            ProductRepository reposiroty = new ProductRepository();
            int quantity;

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
                reposiroty.UpdateStock(productInfo);
                Console.WriteLine($"{productInfo.Name}を{quantity}個追加しました。");

            }
        }

        public void RemoveProduct(ProductService service, Order order)
        {
            ProductRepository reposiroty = new ProductRepository();
            int quantity;

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
                    reposiroty.UpdateStock(removeItem.Product);
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
        }

        public void ShowOrder(Order order)
        {
            Console.WriteLine();
            Console.WriteLine("注文内容:");
            foreach (var item in order.Items)
            {
                Console.WriteLine($"商品: {item.Product.Name}, 数量: {item.Quantity}, 小計: {item.Subtotal}円");
            }
            Console.WriteLine($"合計: {order.Total}円");
        }
    }
}
