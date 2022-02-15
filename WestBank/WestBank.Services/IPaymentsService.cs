using System;
using WestBank.Models;

namespace WestBank.Services
{
    public interface IPaymentsService
    {
        PaymentResponse Process(PaymentRequest paymentRequest);
        PaymentDetails GetDetails(Guid paymentId);
    }
}