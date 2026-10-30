using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Data.Sqlite;
using POSSystem.Domain.Entities;
using POSSystem.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using System.Security.Principal;

namespace POSSystem.Application.Services
{
    public class AccountService
    {
        private readonly AccountRepository _repository;
        private readonly PasswordHasher<Account> _passwordHasher;

        public AccountService (AccountRepository repository)
        {
            _repository = repository;
            _passwordHasher = new PasswordHasher<Account>();
        }

        // ログイン用メソッド
        // usernameとpasswordを受け取り、該当するAccountを返す

        public Account? Login(string username, string password)
        {
            var account = _repository.GetAccount(username);

            if (account == null)
            {
                return null;
            }

            var result = _passwordHasher.VerifyHashedPassword(account, account.PasswordHash, password);
            if (result == PasswordVerificationResult.Failed)
            {
                return null;
            }

            return account;
        }

        // アカウント登録用メソッド
        public void Resister(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new InvalidOperationException("ユーザー名を入力してください。");
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                throw new InvalidOperationException("パスワードを入力してください。");
            }


            //ユーザー名の重複チェック
            var exstingAccount = _repository.GetAccount(username);

            if (exstingAccount != null)
            {
                throw new InvalidOperationException("そのユーザー名は既に使用されています。");
            }

            var dummyAccount = new Account(0, username, "", false);

            string passwordHash = _passwordHasher.HashPassword(dummyAccount, password);

            var account = new Account(0, username, passwordHash, false);

            _repository.AddAccount(account);
        }


        /// <summary>
        /// パスワードを変更するメソッド
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="currentPassword"></param>
        /// <param name="newPassword"></param>
        public void ChangePassword(int accountId, string currentPassword, string newPassword)
        {
            //入力チェック
            if(string.IsNullOrWhiteSpace(currentPassword))
            {
                throw new InvalidOperationException("現在のパスワードを入力してください。");
            }

            if(string.IsNullOrWhiteSpace(newPassword))
            {
                throw new InvalidOperationException("新しいパスワードを入力してください。");
            }

            if(currentPassword == newPassword)
            {
                throw new InvalidOperationException("現在のパスワードと異なるパスワードにしてください。");
            }


            //アカウントを取得する
            var account = _repository.GetAccountById(accountId);

            if(account == null)
            {
                throw new InvalidOperationException("アカウントが見つかりません。");
            }


            //現在のパスワードを検証
            var result = _passwordHasher.VerifyHashedPassword(account, account.PasswordHash, currentPassword);

            if (result == PasswordVerificationResult.Failed)
            {
                throw new InvalidOperationException("現在のパスワードが正しくありません。");
            }


            //新しいパスワードをハッシュ化
            string newPasswordHash = _passwordHasher.HashPassword(account, newPassword);

            //DBを更新する
            _repository.UpdatePasswordHash(accountId, newPasswordHash);
        }
    }
}
