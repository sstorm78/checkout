using System;

namespace Checkout.Models
{
    public class PaymentRequestModel
    {
        public string PaymentCardNumber { get; set; }
        public DateTime ExpiryDate { get; set; }
        public int CvvNumber { get; set; }
        public decimal Amount { get; set; }
        public string CurrencyCode { get; set; }
    }
}
