using POSSystem.Domain.Entities;
using POSSystem.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace POSSystem.Application.Services
{
    public class CheckoutService
    {
        private readonly SaleRepository _saleRepository;

        public CheckoutService(SaleRepository saleRepository)
        {
            _saleRepository = saleRepository;
        }

        public void Checkout(Order order)
        {
            decimal payment;
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
        }
    }
}
