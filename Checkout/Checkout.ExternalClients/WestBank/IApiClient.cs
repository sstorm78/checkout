using Checkout.ExternalClients.WestBank.Models;
using System;
using System.Threading.Tasks;

namespace Checkout.ExternalClients.WestBank
{
    public interface IApiClient
    {
        Task<PaymentDetails> GetTransactionDetails(Guid paymentId);
        Task<PaymentResponse> Pay(PaymentRequest paymentRequest);
    }
}