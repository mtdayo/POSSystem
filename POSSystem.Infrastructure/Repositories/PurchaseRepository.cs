using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace POSSystem.Infrastructure.Repositories
{
    public class PurchaseRepository
    {
        private const string ConnectionString = @"Data Source=C:\Users\lawri\Documents\GitHub\POSSystem\POSSystem.Infrastructure\Database\pos.db";

        private sealed record PurchaseLine(string ProductCode, string ProductName, decimal Price, int Quantity, int Stock)
        {
            public decimal SubTotal => Price * Quantity;
        }

        public int CompletePurchase(int accountId)
        {
            using var connection = new SqliteConnection(ConnectionString);

            connection.Open();

            using var transaction = connection.BeginTransaction();

            try
            {
                //ログインユーザーのカートを取得
                int cartId;

                const string cartSql = """
                    SELECT Id 
                    FROM Cart
                    WHERE AccountId = @AccountId;
                    """;

                using (var command = new SqliteCommand(cartSql, connection, transaction))
                {
                    command.Parameters.AddWithValue("@AccountId", accountId);

                    var result = command.ExecuteScalar();

                    if (result == null || result == DBNull.Value) throw new InvalidOperationException("カートが見つかりません。");

                    cartId = Convert.ToInt32(result);
                }

                var lines = new List<PurchaseLine>();

                const string itemSql = """
                    SELECT 
                        p.Code,
                        p.Name,
                        p.Price,
                        ci.Quantity,
                        p.Stock
                    FROM CartItems ci
                    INNER JOIN Products p
                        ON p.Code = ci.ProductCode
                    WHERE ci.CartId = @CartId;
                    """;

                using (var command = new SqliteCommand(itemSql, connection, transaction))
                {
                    command.Parameters.AddWithValue("@CartId", cartId);

                    using var reader = command.ExecuteReader();

                    while(reader.Read())
                    {
                        lines.Add(new PurchaseLine(
                            reader.GetString(0),
                            reader.GetString(1),
                            reader.GetDecimal(2),
                            reader.GetInt32(3),
                            reader.GetInt32(4)
                            ));
                    }
                }

                if(lines.Count == 0)
                    throw new InvalidOperationException("カートに商品がありません。");


                //商品の存在・数量・在庫を確認
                foreach(var line in lines)
                {
                    if (line.Quantity <= 0) throw new InvalidOperationException($"{line.ProductName}の購入数量が不正です。");

                    if (line.Stock < line.Quantity) throw new InvalidOperationException($"{line.ProductName}の在庫が足りません。現在の在庫 : {line.Stock}");
                }


                decimal totalAmount = 0;

                foreach (var line in lines)
                {
                    totalAmount += line.SubTotal;
                }

                const string saleSql = """
                    INSERT INTO Sales (AccountId, SaleDate, TotalAmount) 
                    VALUES (@AccountId, @SaleDate, @TotalAmount);
                    """;

                using (var command = new SqliteCommand(saleSql, connection, transaction))
                {
                    command.Parameters.AddWithValue("@AccountId", accountId);
                    command.Parameters.AddWithValue("@SaleDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    command.Parameters.AddWithValue("@TotalAmount", totalAmount);

                    command.ExecuteNonQuery();
                }

                int saleId;

                using (var command = new SqliteCommand("SELECT last_insert_rowid();", connection, transaction))
                {
                    saleId = Convert.ToInt32(command.ExecuteScalar());
                }


                //SaleItemsに購入明細を登録する
                const string saleItemSql = """
                    INSERT INTO SaleItems
                    (SaleId, ProductCode, ProductName, Price, Quantity, SubTotal)
                    VALUES(@SaleId, @ProductCode, @ProductName, @Price, @Quantity, @SubTotal);
                    """;

                foreach(var line in lines)
                {
                    using var command = new SqliteCommand(saleItemSql, connection, transaction);

                    command.Parameters.AddWithValue("@SaleId", saleId);
                    command.Parameters.AddWithValue("@ProductCode", line.ProductCode);
                    command.Parameters.AddWithValue("@ProductName", line.ProductName);
                    command.Parameters.AddWithValue("@Price", line.Price);
                    command.Parameters.AddWithValue("@Quantity", line.Quantity);
                    command.Parameters.AddWithValue("@SubTotal", line.SubTotal);

                    command.ExecuteNonQuery();
                }

                const string stockSql = """
                    UPDATE Products
                    SET Stock = Stock - @Quantity
                    WHERE Code = @ProductCode
                        AND Stock >= @Quantity;
                    """;

                foreach(var line in lines)
                {
                    using var command = new SqliteCommand(stockSql, connection, transaction);

                    command.Parameters.AddWithValue("@Quantity", line.Quantity);
                    command.Parameters.AddWithValue("@ProductCode", line.ProductCode);

                    if(command.ExecuteNonQuery() != 1)
                    {
                        throw new InvalidOperationException($"{line.ProductName}の在庫が足りません。");
                    }
                    
                }

                //購入成功時にカートを空にする
                const string clearCartSql = """
                    DELETE FROM CartItems
                    WHERE CartId = @CartId;
                    """;

                using (var command = new SqliteCommand(clearCartSql, connection, transaction))
                {
                    command.Parameters.AddWithValue("@CartId", cartId);
                    command.ExecuteNonQuery();
                }


                //すべて成功したら確定
                transaction.Commit();

                return saleId;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}
