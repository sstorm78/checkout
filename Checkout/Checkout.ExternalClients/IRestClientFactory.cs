using RestSharp;

namespace Checkout.ExternalClients
{
    public interface IRestClientFactory
    {
        RestClient GetClient();
    }
}