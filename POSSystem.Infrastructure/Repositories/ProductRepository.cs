using POSSystem.Domain.Entities;
using Microsoft.Data.Sqlite;

namespace POSSystem.Infrastructure.Repositories
{
    public class ProductRepository
    {
        private const string ConnectionString = @"Data Source=C:\Users\lawri\Documents\GitHub\POSSystem\POSSystem.Infrastructure\Database\pos.db";


        public IReadOnlyList<Product> GetAllProducts()
        {
            List<Product> products = new();

            using var connection = new SqliteConnection(ConnectionString);

            connection.Open();

            string sql = """
                SELECT
                    Id,
                    Code,
                    Name,
                    Price,
                    Stock
                FROM Products
                """;

            using var command = new SqliteCommand(sql, connection);

            using var reader = command.ExecuteReader();

            while(reader.Read())
            {
                products.Add(new Product(
                    reader.GetInt32(0),
                    reader.GetString(1),
                    reader.GetString(2),
                    reader.GetDecimal(3),
                    reader.GetInt32(4)));
            }

            return products;
        }

        public Product? GetProductByCode(string code)
        {
            using var connection = new SqliteConnection(ConnectionString);

            connection.Open();

            string sql = """
                SELECT
                    Id,
                    Code,
                    Name,
                    Price,
                    Stock
                FROM Products
                WHERE Code = @Code
                """;

            using var command = new SqliteCommand(sql, connection);
            command.Parameters.AddWithValue("@Code", code);

            using var reader = command.ExecuteReader();

            if (reader.Read())
            {
                return new Product(
                    reader.GetInt32(0),
                    reader.GetString(1),
                    reader.GetString(2),
                    reader.GetDecimal(3),
                    reader.GetInt32(4));
            }

            return null;
        }

        public void UpdateStock(Product product)
        {
            using var connection = new SqliteConnection(ConnectionString);

            connection.Open();

            string sql = """
                UPDATE Products
                SET Stock = @stock
                WHERE Code = @code
                """;

            using var command = new SqliteCommand(sql, connection);

            command.Parameters.AddWithValue("@stock", product.Stock);
            command.Parameters.AddWithValue("@code", product.Code);

            command.ExecuteNonQuery();
        }
    }
}
