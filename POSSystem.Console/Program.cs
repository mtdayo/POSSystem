using POSSystem.Application.Services;
using POSSystem.Domain.Entities;
using POSSystem.Infrastructure.Repositories;

ProductRepository repository = new ProductRepository();
ProductService service = new ProductService(repository);
Order order = new Order();
OrderService orderservice = new OrderService();
SaleRepository saleRepository = new SaleRepository();
CheckoutService checkoutService = new CheckoutService(saleRepository);


while (true)
{
    Console.WriteLine("1 : 商品一覧を表示");
    Console.WriteLine("2 : 商品を追加");
    Console.WriteLine("3 : 商品を削除");
    Console.WriteLine("4 : 注文一覧");
    Console.WriteLine("5 : 会計");
    Console.WriteLine("6 : 売上一覧");
    Console.WriteLine("0 : 終了");
    Console.WriteLine();
    Console.Write("番号を入力してください: ");

    switch (Console.ReadLine())
    {
        case "1":
            service.ShowAllProduct(service);
            break;
        case "2":
            orderservice.AddProduct(service, order);
            break;
        case "3":
            orderservice.RemoveProduct(service, order);
            break;
        case "4":
            orderservice.ShowOrder(order);
            break;
        case "5":
            checkoutService.Checkout(order);
            Sale sale = new Sale(
                0,
                DateTime.Now,
                order.Items,
                order.Total
            );
            saleRepository.AddSale(sale);
            order.Clear();
            break;
        case "6":
            var sales = saleRepository.GetAllSales();
            foreach (var s in sales)
            {
                Console.WriteLine($"Sale ID: {s.Id}, Date: {s.SaleDate}, Total: {s.TotalAmount}");
            }
            break;
        case "0":
            return;
        default:
            Console.WriteLine("無効な入力です。");
            break;
    }
}