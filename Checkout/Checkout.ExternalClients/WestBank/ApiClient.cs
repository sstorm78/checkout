using Checkout.ExternalClients.WestBank.Models;
using RestSharp;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Checkout.ExternalClients.WestBank
{
    public class ApiClient : ApiClientBase, IApiClient
    {
        private readonly IRestClientFactory _restClientFactory;

        public ApiClient(string baseUrl, IRestClientFactory restClientFactory)
            :base(baseUrl)
        {
            _restClientFactory = restClientFactory;
        }

        public async Task<PaymentResponse> Pay(PaymentRequest paymentRequest)
        {
            var restClient = _restClientFactory.GetClient();

            var url = "payments";

            var result = await Post<RestResponse<string>>(restClient, url, paymentRequest);

            if (result.StatusCode == System.Net.HttpStatusCode.Created)
            {
                var paymentId = result.Headers.First(i => i.Name == "Location").Value.ToString().Split('/').Last();

                string warning = string.Empty;

                if (!string.IsNullOrEmpty(result.Content))
                {
                    warning = JsonSerializer.Deserialize<string>(result.Content);
                }

                if (!string.IsNullOrEmpty(warning))
                {
                    return new PaymentResponse()
                    .SuccessWithWarning(new Guid(paymentId), warning);
                }

                return new PaymentResponse()
                    .Success(new Guid(paymentId));
            }

            return new PaymentResponse().Decline(result.StatusDescription);
        }

        public async Task<PaymentDetails> GetTransactionDetails(Guid paymentId)
        {
            var restClient = _restClientFactory.GetClient();
            var url = $"payments/{paymentId}";

            return await Get<PaymentDetails>(restClient, url);
        }
    }
}
