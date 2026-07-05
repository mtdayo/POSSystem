using POSSystem.Application.Services;
using POSSystem.Domain.Entities;
using POSSystem.Infrastructure.Repositories;

ProductRepository repository = new ProductRepository();
ProductService service = new ProductService(repository);
Order order = new Order();
OrderService orderservice = new OrderService();
CheckoutService checkoutService = new CheckoutService();

orderservice.showAllProduct(service);


orderservice.AddProduct(service, order);

orderservice.RemoveProduct(service, order);

orderservice.showOrder(order);

checkoutService.Checkout(order);
