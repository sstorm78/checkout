using System;
using System.Threading.Tasks;
using Checkout.Models;

namespace Checkout.Services
{
    public interface IPaymentProcessingService
    {
        Task<PaymentResponseModel> Process(PaymentRequestModel paymentRequest);
        Task<PaymentDetailsModel> GetDetails(Guid paymentId);
    }
}