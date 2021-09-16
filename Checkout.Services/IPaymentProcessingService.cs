using System;
using Checkout.Models;

namespace Checkout.Services
{
    public interface IPaymentProcessingService
    {
        PaymentResponseModel Process(PaymentRequestModel paymentRequest);
        PaymentDetailsModel GetDetails(Guid paymentId);
    }
}