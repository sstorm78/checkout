using System;

namespace Checkout.Models
{
    public class PaymentResponseModel
    {
        public Guid? PaymentId { get; set; }
        public string Status { get; set; }
        public string Reason { get; set; }
    }
}
