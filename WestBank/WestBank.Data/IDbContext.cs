using System.Collections.Generic;

namespace WestBank.Data
{
    public interface IDbContext
    {
        List<Entities.Payment> Payments { get; set; }
    }
}