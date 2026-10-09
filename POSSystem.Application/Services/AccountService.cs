using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Data.Sqlite;
using POSSystem.Domain.Entities;
using POSSystem.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;

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
        public void Resister(string username, string password, bool isEmployee)
        {

            //ユーザー名の重複チェック
            var exstingAccount = _repository.GetAccount(username);

            if (exstingAccount != null)
            {
                throw new Exception("そのユーザー名は既に使用されています。");
            }

            var dummyAccount = new Account(0, username, "", isEmployee);

            string passwordHash = _passwordHasher.HashPassword(dummyAccount, password);

            var account = new Account(0, username, passwordHash, isEmployee);

            _repository.AddAccount(account);
        }
    }
}
