using System;
using System.Collections.Generic;
using System.Text;

namespace POSSystem.Domain.Entities
{
    public class Cart
    {
        public int Id { get; set; }
        public int AccountId { get; set; } //カートを所有するアカウントID
    }
}
