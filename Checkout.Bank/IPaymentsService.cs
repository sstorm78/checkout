using System;
using Checkout.Bank.Models;

namespace Checkout.Bank
{
    public interface IPaymentsService
    {
        PaymentResponse Process(PaymentRequest paymentRequest);
        PaymentDetails GetDetails(Guid paymentId);
    }
}