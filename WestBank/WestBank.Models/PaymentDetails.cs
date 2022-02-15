using System;
using System.Text.RegularExpressions;

namespace WestBank.Models
{
    public class PaymentDetails
    {
        public Guid PaymentId { get; }
        public string PaymentCardNumber { get; }
        
        public decimal Amount { get; }
        public DateTime DateTime { get; }

        public string Status { get; }

        public string CurrencyCode { get; }

        public PaymentDetails(Guid paymentId, string paymentCardNumber, decimal amount, DateTime dateTime, string status, string currencyCode)
        {
            PaymentId = paymentId;
            PaymentCardNumber = Regex.Replace(paymentCardNumber, "[0-9](?=[0-9]{4})", "*");
            Amount = amount;
            DateTime = dateTime;
            Status = status;
            CurrencyCode = currencyCode;
        }

    }
}
