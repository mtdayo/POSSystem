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

        public Account(int id, string username, string passwordHash, bool isEmployee)
        {
            Id = id;
            Username = username;
            PasswordHash = passwordHash;
            IsEmployee = isEmployee;
        }
    }
}
