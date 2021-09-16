using System;

namespace Checkout.Bank.Models
{
    public  class PaymentResponse
    {
        public Guid? PaymentId { get; private set; }
        public PaymentStatus Status { get; private set; }
        public string Reason { get; private set; }

        public PaymentResponse()
        {
        }
        
        public PaymentResponse Success(Guid paymentId)
        {
            PaymentId = paymentId;
            Status = PaymentStatus.Success;
            return this;
        }

        public PaymentResponse Decline(string reason)
        {
            Status = PaymentStatus.Declined;
            Reason = reason;
            return this;
        }

        public PaymentResponse SuccessWithWarning(Guid paymentId, string reason)
        {
            PaymentId = paymentId;
            Status = PaymentStatus.SuccessWithWarning;
            Reason = reason;
            return this;
        }
    }
}
