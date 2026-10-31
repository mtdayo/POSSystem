using System;
using System.Collections.Generic;
using System.Text;

namespace POSSystem.Domain.Entities
{
    public class Account
    {
        public int Id { get; }
        public string Username { get; }
        public string PasswordHash { get; }
        public bool IsEmployee { get; }

        public decimal CashBalance { get; }
        public decimal ElectronicMoneyBalance { get; }

        public Account(int id, string username, string passwordHash, bool isEmployee, decimal cashBalnce, decimal electronicMoneyBalance)
        {
            Id = id;
            Username = username;
            PasswordHash = passwordHash;
            IsEmployee = isEmployee;
            CashBalance = cashBalnce;
            ElectronicMoneyBalance = electronicMoneyBalance;
        }
    }
}
