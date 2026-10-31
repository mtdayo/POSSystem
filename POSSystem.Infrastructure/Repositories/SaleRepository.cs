//購入履歴・売上　クエリ文

using Microsoft.Data.Sqlite;
using POSSystem.Domain.Entities;
using POSSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Net;
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
                    TotalAmount,
                    Payment
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
                    reader.GetDecimal(3),
                    Enum.Parse<Payment>(reader.GetString(4))
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

        /// <summary>
        /// アカウントIDから購入履歴を取得
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public IReadOnlyList<Sale> GetSalesByAccountId(int accountId)
        {
            List<Sale> sales = new();

            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            string sql = """
                SELECT Id, AccountId, SaleDate, TotalAmount, Payment
                FROM Sales
                WHERE AccountId = @AccountId
                ORDER BY SaleDate DESC;
                """;

            using var command = new SqliteCommand(sql, connection);
            command.Parameters.AddWithValue("@AccountId", accountId);

            using var reader = command.ExecuteReader();

            while(reader.Read())
            {
                sales.Add(new Sale(
                    reader.GetInt32(0),
                    reader.GetInt32(1),
                    DateTime.Parse(reader.GetString(2)),
                    new List<SaleItem>(),
                    reader.GetDecimal(3),
                    Enum.Parse<Payment>(reader.GetString(4))));
                    
            }

            return sales;
        }

        public IReadOnlyList<SaleItem> GetSaleItemsByAccountId(int saleId, int accountId)
        {
            List<SaleItem> items = new();

            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            string sql = """
                SELECT
                    si.Id,
                    si.SaleId,
                    si.ProductCode,
                    si.ProductName,
                    si.Price,
                    si.Quantity,
                    si.SubTotal
                FROM SaleItems si
                INNER JOIN Sales s ON si.SaleId = s.Id
                WHERE s.Id = @SaleId
                AND s.AccountId = @AccountId
                ORDER BY si.Id;
                """;

            using var command = new SqliteCommand(sql, connection);
            command.Parameters.AddWithValue("@SaleId", saleId);
            command.Parameters.AddWithValue("@AccountId", accountId);

            using var reader = command.ExecuteReader();

            while(reader.Read())
            {
                items.Add(new SaleItem(
                    reader.GetInt32(0),
                    reader.GetInt32(1),
                    reader.GetString(2),
                    reader.GetString(3),
                    reader.GetDecimal(4),
                    reader.GetInt32(5),
                    reader.GetDecimal(6)));
            }

            return items;
        }

        public decimal GetTotalSales(DateTime startDate, DateTime endDate)
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            string sql = """
                SELECT COALESCE(SUM(TotalAmount), 0)
                FROM Sales
                WHERE SaleDate >= @StartDate AND SaleDate < @EndDate;
                """;

            using var command = new SqliteCommand(sql, connection);
            command.Parameters.AddWithValue("@StartDate", startDate.ToString("yyyy-MM-dd HH:mm:ss"));
            command.Parameters.AddWithValue("@EndDate", endDate.ToString("yyyy-MM-dd HH:mm:ss"));

            return Convert.ToDecimal(command.ExecuteScalar());

            
        }

        public IReadOnlyList<Sale> GetSalesByPeriod(DateTime startDate, DateTime endDate)
        {
            List<Sale> sales = new();

            using var connection = new SqliteConnection(ConnectionString);

            connection.Open();

            string sql = """
                SELECT
                    Id,
                    AccountId,
                    SaleDate,
                    TotalAmount,
                    Payment
                FROM Sales
                WHERE SaleDate >= @StartDate AND SaleDate < @EndDate
                ORDER BY SaleDate DESC;
                """;

            using var command = new SqliteCommand(sql, connection);
            command.Parameters.AddWithValue("@StartDate", startDate.ToString("yyyy-MM-dd HH:mm:ss"));
            command.Parameters.AddWithValue("@EndDate", endDate.ToString("yyyy-MM-dd HH:mm:ss"));

            using var reader = command.ExecuteReader();

            while(reader.Read())
            {
                sales.Add(new Sale(
                    reader.GetInt32(0),
                    reader.GetInt32(1),
                    DateTime.Parse(reader.GetString(2)),
                    new List<SaleItem>(),
                    reader.GetDecimal(3),
                    Enum.Parse<Payment>(reader.GetString(4))
                ));
            }

            return sales;


        }

        /// <summary>
        /// 従業員ロールが注文IDから購入明細を取得するメソッド
        /// </summary>
        /// <param name="saleId"></param>
        /// <returns></returns>
        public IReadOnlyList<SaleItem> GetSaleItemsForEmployee(int saleId)
        {
            List<SaleItem> items = new();

            using var connection = new SqliteConnection(ConnectionString);

            connection.Open();

            string sql = """
                SELECT
                    Id,
                    SaleId,
                    ProductCode,
                    ProductName,
                    Price,
                    Quantity,
                    SubTotal
                FROM SaleItems
                WHERE SaleId = @SaleId
                ORDER BY Id; 
                """;

            using var command = new SqliteCommand(sql, connection);
            command.Parameters.AddWithValue("@SaleId", saleId);

            using var reader = command.ExecuteReader();

            while(reader.Read())
            {
                items.Add(new SaleItem(
                    reader.GetInt32(0),
                    reader.GetInt32(1),
                    reader.GetString(2),
                    reader.GetString(3),
                    reader.GetDecimal(4),
                    reader.GetInt32(5),
                    reader.GetDecimal(6)
                ));
            }

            return items;
        }

    }
}
