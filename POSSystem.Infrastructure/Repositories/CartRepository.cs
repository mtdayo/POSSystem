using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Text;
using POSSystem.Domain.Entities;

namespace POSSystem.Infrastructure.Repositories
{
    public class CartRepository
    {
        private const string ConnectionString = @"Data Source=C:\Users\lawri\Documents\GitHub\POSSystem\POSSystem.Infrastructure\Database\pos.db";

        /// <summary>
        /// アカウントに対応するカートを取得する
        /// カートが無ければ新規作成する
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public Cart GetCartByAccountId(int accountId)
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            string selectSql = """
                SELECT Id, AccountId
                FROM Cart
                WHERE AccountId = @AccountId
                """;

            using var selectCommand = new SqliteCommand(selectSql, connection);

            selectCommand.Parameters.AddWithValue("@AccountId", accountId);

            using var reader = selectCommand.ExecuteReader();

            if(reader.Read())
            {
                return new Cart
                {
                    Id = reader.GetInt32(0),
                    AccountId = reader.GetInt32(1)
                };
            }

            reader.Close();

            string insertSql = """
                INSERT INTO Cart (AccountId)
                VALUES (@AccountId)
                """;

            using var insertCommand = new SqliteCommand(insertSql, connection);

            insertCommand.Parameters.AddWithValue("@AccountId", accountId);
            insertCommand.ExecuteNonQuery();

            string idSql = "SELECT last_insert_rowid();";

            using var idCommand = new SqliteCommand(idSql, connection);

            int cartID = (int)(long)idCommand.ExecuteScalar()!;

            return new Cart
            {
                Id = cartID,
                AccountId = accountId
            };

            //return new Cart
            //{
            //    Id = (int)connection.LastInsertRowId,
            //    AccountId = accountId
            //};
        }

        /// <summary>
        /// カート内の商品と数量を取得する
        /// </summary>
        /// <param name="cartId"></param>
        /// <returns></returns>
        public List<CartItem> GetItemsByCartId(int cartId)
        {
            List<CartItem> items = new();

            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            string sql = """
                SELECT Id, CartId, ProductCode, Quantity
                FROM CartItems
                WHERE CartId = @CartId
                """;

            using var command = new SqliteCommand(sql, connection);
            command.Parameters.AddWithValue("@CartId", cartId);

            using var reader = command.ExecuteReader();

            while(reader.Read())
            {
                items.Add(new CartItem
                {
                    Id = reader.GetInt32(0),
                    CartId = reader.GetInt32(1),
                    ProductCode = reader.GetString(2),
                    Quantity = reader.GetInt32(3)
                });
            }


            return items;
        }

        /// <summary>
        /// 商品を追加する
        /// 同じ商品があれば数量を加算する
        /// </summary>
        /// <param name="cartId"></param>
        /// <param name="productCode"></param>
        /// <param name="quantity"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public void AddItem(int cartId, string productCode, int quantity)
        {
            if(quantity <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(quantity), "数量は１個以上にしてください。");
            }

            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            using var transaction = connection.BeginTransaction();

            string updateSql = """
                UPDATE CartItems
                SET Quantity = Quantity + @Quantity
                WHERE CartId = @CartId
                AND ProductCode = @ProductCode
                """;

            using var updateCommand = new SqliteCommand(updateSql, connection, transaction);

            updateCommand.Parameters.AddWithValue("@Quantity", quantity);
            updateCommand.Parameters.AddWithValue("@CartId", cartId);
            updateCommand.Parameters.AddWithValue("@ProductCode", productCode);

            int rowsAffected = updateCommand.ExecuteNonQuery();

            if(rowsAffected == 0)
            {
                string insertSql = """
                    INSERT INTO CartItems (CartId, ProductCode, Quantity)
                    VALUES (@CartId, @ProductCode, @Quantity)
                    """;

                using var insertCommand = new SqliteCommand(insertSql, connection, transaction);

                insertCommand.Parameters.AddWithValue("@CartId", cartId);
                insertCommand.Parameters.AddWithValue("@ProductCode", productCode);
                insertCommand.Parameters.AddWithValue("@Quantity", quantity);

                insertCommand.ExecuteNonQuery();
            }

            transaction.Commit();
        }

        /// <summary>
        /// カート内の商品の数量を変更する
        /// </summary>
        /// <param name="cartItemId"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        public bool UpdateQuantity(int cartItemId, int quantity)
        {
            if(quantity <= 0)
            {
                return false;
            }

            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            string sql = """
                UPDATE CartItems
                SET Quantity = @Quantity
                WHERE Id = @Id
                """;

            using var command = new SqliteCommand(sql, connection);

            command.Parameters.AddWithValue("@Quantity", quantity);
            command.Parameters.AddWithValue("@Id", cartItemId);

            return command.ExecuteNonQuery() > 0;
        }


        public bool RemoveItem(int cartItemId)
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open ();

            string sql = """
                DELETE FROM CartItems
                WHERE Id = @Id
                """;
            
            using var command = new SqliteCommand (sql, connection);

            command.Parameters.AddWithValue("@Id", cartItemId);

            return command.ExecuteNonQuery () > 0;
        }


        /// <summary>
        /// カートを空にするメソッド
        /// </summary>
        /// <param name="cartId"></param>
        public void ClearItems(int cartId)
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            string sql = """
                DELETE FROM CartItems
                WHERE CartId = @CartId
                """;

            using var command = new SqliteCommand(sql, connection);
            command.Parameters.AddWithValue("@CartId", cartId);

            command.ExecuteNonQuery();
        }
    }
}
