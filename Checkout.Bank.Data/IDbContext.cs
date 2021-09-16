using System.Collections.Generic;

namespace Checkout.Bank.Data
{
    public interface IDbContext
    {
        List<Entities.Payment> Payments { get; set; }
    }
}