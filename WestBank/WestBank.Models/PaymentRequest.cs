using System;

namespace WestBank.Models
{
    public class PaymentRequest
    {
        public string PaymentCardNumber { get; set; }
        public DateTime ExpiryDate { get; set; }
        public int CvvNumber { get; set; }
        public decimal Amount { get; set; }
        public string CurrencyCode { get; set; }
    }
}
