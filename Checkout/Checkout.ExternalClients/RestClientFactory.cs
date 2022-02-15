using RestSharp;

namespace Checkout.ExternalClients
{
    public class RestClientFactory : IRestClientFactory
    {
        private RestClient restClient;

        public RestClient GetClient()
        {
            if (restClient == null)
            {
                restClient = new RestClient();
            }

            return restClient;
        }

    }
}
