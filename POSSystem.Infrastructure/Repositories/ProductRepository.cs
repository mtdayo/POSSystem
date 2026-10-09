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

        public void AddProduct (Product product)
        {
            using var connection = new SqliteConnection(ConnectionString);

            connection.Open();

            string sql = """
                INSERT INTO Products (Code, Name, Price, Stock)
                VALUES (@code, @name, @price, @stock)
                """;
            using var command = new SqliteCommand(sql, connection);
            command.Parameters.AddWithValue("@code", product.Code);
            command.Parameters.AddWithValue("@name", product.Name);
            command.Parameters.AddWithValue("@price", product.Price);
            command.Parameters.AddWithValue("@stock", product.Stock);
            command.ExecuteNonQuery();
        }

        public bool UpdateProduct(Product product)
        {
            using var connection = new SqliteConnection(ConnectionString);

            connection.Open();

            string sql = """
                UPDATE Products
                SET
                    Name = @name,
                    Price = @price,
                    Stock = @stock
                WHERE Code = @code
                """;

            using var command = new SqliteCommand(sql, connection);

            command.Parameters.AddWithValue("@name", product.Name);
            command.Parameters.AddWithValue("@price", product.Price);
            command.Parameters.AddWithValue("@stock", product.Stock);
            command.Parameters.AddWithValue("@code", product.Code);

            int rowsAffected = command.ExecuteNonQuery();

            return rowsAffected > 0;
        }

        public bool DeleteProduct(string code)
        {
            using var connection = new SqliteConnection(ConnectionString);

            connection.Open();

            string sql = """
                DELETE FROM Products
                WHERE Code = @code
                """;

            using var command = new SqliteCommand(sql, connection);
            command.Parameters.AddWithValue("@code", code);
            int rowsAffected = command.ExecuteNonQuery();
            return rowsAffected > 0;
        }

        public List<Product> SearchProducts(string? search, string sortBy, string sortOrder)
        {
            List<Product> products = new();
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            string sql = $"""
                SELECT
                    Id,
                    Code,
                    Name,
                    Price,
                    Stock
                FROM Products
                """;
            if (!string.IsNullOrEmpty(search))
            {
                sql += " WHERE Name LIKE @search";
            }

            sql += sortBy.ToLower() switch
            {
                "name" => " ORDER BY Name",
                "price" => " ORDER BY Price",
                "stock" => " ORDER BY Stock",
                _ => " ORDER BY Code"
            };

            if (sortOrder.ToLower() == "desc")
            {
                sql += " DESC";
            }

            using var command = new SqliteCommand(sql, connection);

            if (!string.IsNullOrEmpty(search))
            {
                command.Parameters.AddWithValue("@search", $"%{search}%");
            }

            using var reader = command.ExecuteReader();

            //var products = new List<Product>();

            while (reader.Read())
            {
                products.Add(new Product
                (
                    reader.GetInt32(0),
                    reader.GetString(1),
                    reader.GetString(2),
                    reader.GetDecimal(3),
                    reader.GetInt32(4)
                ));
            }

            return products;
        }
    }
}
