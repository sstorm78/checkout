using System;

namespace Checkout.Bank.Data.Entities
{
    public class Payment
    {
        public Guid Id { get; set; }
        public string PaymentCardNumber { get; set; }
        public decimal Amount { get; set; }
        public DateTime DateTime { get; set; }
        public string Status { get; set; }
        public string CurrencyCode { get; set; }

    }
}
