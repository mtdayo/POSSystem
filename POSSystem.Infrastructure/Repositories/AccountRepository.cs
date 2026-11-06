using POSSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Data.Sqlite;
using System.Runtime.InteropServices.Marshalling;


namespace POSSystem.Infrastructure.Repositories
{
    public class AccountRepository
    {
        private const string ConnectionString = @"Data Source=C:\Users\lawri\Documents\GitHub\POSSystem\POSSystem.Infrastructure\Database\pos.db";


        //ユーザー名からアカウントを取得するメソッド
        public Account? GetAccount(string username)
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();


            string sql = """
                SELECT
                    Id, Username, PasswordHash, IsEmployee, CashBalance, ElectronicMoneyBalance
                FROM Accounts
                WHERE Username = @Username
                """;

            using var command = new SqliteCommand(sql, connection);
            command.Parameters.AddWithValue("@Username", username);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return new Account
                (
                    reader.GetInt32(0),
                    reader.GetString(1),
                    reader.GetString(2),
                    reader.GetBoolean(3),
                    reader.GetDecimal(4),
                    reader.GetDecimal(5)
                );
            }

            return null;
        }

        //アカウント情報登録用メソッド
        public void AddAccount(Account account)
        {
            using var connenction = new SqliteConnection(ConnectionString);
            connenction.Open();

            string sql = """
                INSERT INTO Accounts
                (
                    Username, PasswordHash, IsEmployee
                )
                VALUES
                (
                    @Username, @PasswordHash, @IsEmployee
                )
                """;

            using var command = new SqliteCommand(sql, connenction);

            command.Parameters.AddWithValue("@Username", account.Username);
            command.Parameters.AddWithValue("@PasswordHash", account.PasswordHash);
            command.Parameters.AddWithValue("@IsEmployee", account.IsEmployee ? 1 : 0);

            command.ExecuteNonQuery();
        }


        /// <summary>
        /// IDからアカウントを取得するメソッド
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public Account? GetAccountById(int accountId)
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            string sql = """
                SELECT Id, Username, PasswordHash, IsEmployee, CashBalance, ElectronicMoneyBalance
                FROM Accounts
                WHERE Id = @Id;
                """;

            using var command = new SqliteCommand(sql, connection);

            command.Parameters.AddWithValue("@Id", accountId);

            using var reader = command.ExecuteReader();

            if(reader.Read())
            {
                return new Account(
                    reader.GetInt32(0),
                    reader.GetString(1),
                    reader.GetString(2),
                    reader.GetBoolean(3),
                    reader.GetDecimal(4),
                    reader.GetDecimal(5)
                );
            }

            return null;
        }


        /// <summary>
        /// パスワードハッシュを更新するメソッド
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="passwordHash"></param>
        public void UpdatePasswordHash(int accountId, string passwordHash)
        {
            using var connection = new SqliteConnection(ConnectionString);

            connection.Open();

            string sql = """
                UPDATE Accounts
                SET PasswordHash = @PasswordHash
                WHERE Id = @Id;
                """;

            using var command = new SqliteCommand(sql, connection);

            command.Parameters.AddWithValue("@PasswordHash", passwordHash);
            command.Parameters.AddWithValue("@Id", accountId);

            int affectedRows = command.ExecuteNonQuery();

            if (affectedRows != 1)
            {
                throw new InvalidOperationException("パスワードを更新できませんでした。");
            }
        }

    }
}
