using System;

namespace Checkout.Models
{
    public class PaymentDetailsModel
    {
        public Guid PaymentId { get; set; }
        public string PaymentCardNumber { get; set; }

        public decimal Amount { get; set; }
        public DateTime DateTime { get; set; }

        public string Status { get; set; }

        public string CurrencyCode { get; set; }
    }
}
