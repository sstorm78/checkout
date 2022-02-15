using Checkout.ExternalClients.WestBank;
using Checkout.ExternalClients.WestBank.Models;
using Checkout.Models;
using System;
using System.Threading.Tasks;

namespace Checkout.Services
{
    public class PaymentProcessingService : IPaymentProcessingService
    {
        private readonly IApiClient _apiClient;

        public PaymentProcessingService(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<PaymentResponseModel> Process(PaymentRequestModel paymentRequest)
        {
            var request = new PaymentRequest
            {
                PaymentCardNumber = paymentRequest.PaymentCardNumber,
                ExpiryDate = paymentRequest.ExpiryDate,
                CurrencyCode = paymentRequest.CurrencyCode,
                CvvNumber = paymentRequest.CvvNumber,
                Amount = paymentRequest.Amount
            };

            var result = await _apiClient.Pay(request);

            // perform logging, audit and etc

            return new PaymentResponseModel
            {
                PaymentId = result.PaymentId,
                Status = Enum.GetName(typeof(ExternalClients.WestBank.Models.PaymentStatus), result.Status),
                Reason = result.Reason
            };
        }

        public async Task<PaymentDetailsModel> GetDetails(Guid paymentId)
        {
            var result = await _apiClient.GetTransactionDetails(paymentId);

            if (result == null)
            {
                return null;
            }

            return new PaymentDetailsModel
                   {
                       PaymentId = result.PaymentId,
                       Amount = result.Amount,
                       PaymentCardNumber = result.PaymentCardNumber,
                       CurrencyCode = result.CurrencyCode,
                       Status = result.Status,
                       DateTime = result.DateTime
                   };
        }
    }
}
