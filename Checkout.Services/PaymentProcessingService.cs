using System;
using Checkout.Bank;
using Checkout.Bank.Models;
using Checkout.Models;

namespace Checkout.Services
{
    public class PaymentProcessingService : IPaymentProcessingService
    {
        private readonly IPaymentsService _paymentsService;

        public PaymentProcessingService(IPaymentsService paymentsService)
        {
            _paymentsService = paymentsService;
        }

        public PaymentResponseModel Process(PaymentRequestModel paymentRequest)
        {
            var result = _paymentsService.Process(new PaymentRequest
                                            {
                                                PaymentCardNumber = paymentRequest.PaymentCardNumber,
                                                ExpiryDate = paymentRequest.ExpiryDate,
                                                CurrencyCode = paymentRequest.CurrencyCode,
                                                CvvNumber = paymentRequest.CvvNumber,
                                                Amount = paymentRequest.Amount
                                            });

            // perform logging, audit and etc

            return new PaymentResponseModel
                   {
                       PaymentId = result.PaymentId,
                       Status = Enum.GetName(typeof(Bank.Models.PaymentStatus), result.Status),
                       Reason = result.Reason
                   };
        }

        public PaymentDetailsModel GetDetails(Guid paymentId)
        {
            var result = _paymentsService.GetDetails(paymentId);

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
