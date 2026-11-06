using POSSystem.Domain.Entities;
using POSSystem.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace POSSystem.Application.Services
{
    public class OrderService
    {
        private readonly ProductRepository _productRepository;
        
        public OrderService(ProductRepository productRepository)
        {
            _productRepository = productRepository;
        }
        public void AddProduct(Order order, Product product, int quantity)
        {
            if (quantity <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(quantity), 
                    "数量は1以上でなければなりません。");
            }

            if (quantity > product.Stock)
            {
                throw new InvalidOperationException(
                    $"在庫が不足しています。現在の在庫: {product.Stock}");
            }

            order.AddItem(product, quantity);

            product.ReduceStock(quantity);

            _productRepository.UpdateStock(product);
        }

        public void RemoveProduct(Order order,string code,int quantity)
        {
            if (quantity <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(quantity),
                    "数量は1以上でなければなりません。");
            }

            var removedItem = order.RemoveItem(code, quantity);

            if (removedItem == null)
            {
                throw new InvalidOperationException(
                    $"注文に商品コード {code} の商品が存在しません。");
            }

            removedItem.Product.AddStock(quantity);

            _productRepository.UpdateStock(removedItem.Product);
        }

        public IReadOnlyList<OrderItem> GetOrderItems(Order order)
        {
            return order.Items;
        }
    }
}
