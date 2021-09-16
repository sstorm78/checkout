using System.Collections.Generic;
using Checkout.Bank.Data.Entities;

namespace Checkout.Bank.Data
{
    public class DbContext : IDbContext
    {
        public DbContext()
        {
            Payments = new List<Payment>();
        }

        public List<Payment> Payments { get; set; }
    }
}
