using System.Collections.Generic;
using WestBank.Data.Entities;

namespace WestBank.Data
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
