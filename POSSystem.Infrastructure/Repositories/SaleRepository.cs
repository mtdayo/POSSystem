using Microsoft.Data.Sqlite;
using POSSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace POSSystem.Infrastructure.Repositories
{
    public class SaleRepository
    {
        private const string ConnectionString = @"Data Source=C:\Users\lawri\Documents\GitHub\POSSystem\POSSystem.Infrastructure\Database\pos.db";

        public int AddSale(Sale sale)
        {
            using var connection = new SqliteConnection(ConnectionString);

            connection.Open();

            using var transaction = connection.BeginTransaction();

            try
            {
           
                string sql = """
                INSERT INTO Sales
                (
                    AccountId,
                    SaleDate,
                    TotalAmount
                )
                VALUES
                (
                    @AccountId,
                    @SaleDate,
                    @TotalAmount
                );
                """;

                using var command = new SqliteCommand(sql, connection, transaction);

                command.Parameters.AddWithValue("@AccountId", sale.AccountId);
                command.Parameters.AddWithValue("@SaleDate", sale.SaleDate.ToString("yyyy-MM-dd HH:mm:ss"));
                command.Parameters.AddWithValue("@TotalAmount", sale.TotalAmount);

                command.ExecuteNonQuery();


                string idSql = "SELECT last_insert_rowid();";
                using var idCommand = new SqliteCommand(idSql, connection, transaction);

                int saleId = Convert.ToInt32(idCommand.ExecuteScalar());

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
                    using var itemCommand = new SqliteCommand(itemSql, connection, transaction);
                    itemCommand.Parameters.AddWithValue("@SaleId", saleId);
                    itemCommand.Parameters.AddWithValue("@ProductCode", item.ProductCode);
                    itemCommand.Parameters.AddWithValue("@ProductName", item.ProductName);
                    itemCommand.Parameters.AddWithValue("@Price", item.Price);
                    itemCommand.Parameters.AddWithValue("@Quantity", item.Quantity);
                    itemCommand.Parameters.AddWithValue("@SubTotal", item.SubTotal);
                    itemCommand.ExecuteNonQuery();
                }

                transaction.Commit();

                return saleId;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public IReadOnlyList<Sale> GetAllSales()
        {
            List<Sale> sales = new();

            using var connection = new SqliteConnection(ConnectionString);

            connection.Open();

            string sql = """
                SELECT
                    Id,
                    AccountId,
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
                    reader.GetInt32(1),
                    DateTime.Parse(reader.GetString(2)),
                    new List<SaleItem>(),
                    reader.GetDecimal(3)
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
