using Microsoft.Data.Sqlite;
using POSSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace POSSystem.Infrastructure.Repositories
{
    public class SaleRepository
    {
        //private readonly List<Sale> _sales = new List<Sale>();
        private const string ConnectionString = @"Data Source=C:\Users\lawri\Documents\GitHub\POSSystem\POSSystem.Infrastructure\Database\pos.db";

        public void AddSale(Sale sale)
        {
            using var connection = new SqliteConnection(ConnectionString);

            connection.Open();

            string sql = """
                INSERT INTO Sales
                (
                    SaleDate,
                    TotalAmount
                )
                VALUES
                (
                    @SaleDate,
                    @TotalAmount
                );
                """;

            using var command = new SqliteCommand(sql, connection);

            command.Parameters.AddWithValue("@SaleDate", sale.SaleDate.ToString("yyyy-MM-dd HH:mm:ss"));
            command.Parameters.AddWithValue("@TotalAmount", sale.TotalAmount);

            command.ExecuteNonQuery();

            string idSql = "SELECT last_insert_rowid();";
            using var idCommand = new SqliteCommand(idSql, connection);

            long saleId = (long)idCommand.ExecuteScalar();

            foreach (var item in sale.Items)
            {
                string itemSql = """
                    INSERT INTO SaleItems
                    (
                        SaleId,
                        ProductCode,
                        ProductName,
                        Price,
                        Quantity,
                        SubTotal
                    )
                    VALUES
                    (
                        @SaleId,
                        @ProductCode,
                        @ProductName,
                        @Price,
                        @Quantity,
                        @SubTotal
                    );
                    """;
                using var itemCommand = new SqliteCommand(itemSql, connection);
                itemCommand.Parameters.AddWithValue("@SaleId", saleId);
                itemCommand.Parameters.AddWithValue("@ProductCode", item.Product.Code);
                itemCommand.Parameters.AddWithValue("@ProductName", item.Product.Name);
                itemCommand.Parameters.AddWithValue("@Price", item.Product.Price);
                itemCommand.Parameters.AddWithValue("@Quantity", item.Quantity);
                itemCommand.Parameters.AddWithValue("@SubTotal", item.Subtotal);
                itemCommand.ExecuteNonQuery();
            };
        }

        public IReadOnlyList<Sale> GetAllSales()
        {
            List<Sale> sales = new();

            using var connection = new SqliteConnection(ConnectionString);

            connection.Open();

            string sql = """
                SELECT
                    Id,
                    SaleDate,
                    TotalAmount
                FROM Sales
                ORDER BY SaleDate DESC;
                """;

            using var command = new SqliteCommand(sql, connection);

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                sales.Add(new Sale
                (
                    reader.GetInt32(0),
                    DateTime.Parse(reader.GetString(1)),
                    new List<OrderItem>(),
                    reader.GetDecimal(2)
                ));
            }

            return sales;
        }

        public IReadOnlyList<OrderItem> GetSaleItems(int saleId)
        {
            List<OrderItem> items = new();

            using var connection = new SqliteConnection(ConnectionString);

            connection.Open();

            string sql = """
                SELECT
                    ProductCode,
                    ProductName,
                    Price,
                    Quantity
                FROM SaleItems
                WHERE SaleId = @SaleId;
                """;

            using var command = new SqliteCommand(sql, connection);

            command.Parameters.AddWithValue("@SaleId", saleId);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                var product = new Product(
                    0,
                    reader.GetString(0),
                    reader.GetString(1),
                    reader.GetDecimal(2),
                    0
                );

                var orderItem = new OrderItem(product, reader.GetInt32(3));

                items.Add(orderItem);
            }

            return items;
        }
    }
}
