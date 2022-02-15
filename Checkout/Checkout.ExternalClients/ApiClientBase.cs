using Newtonsoft.Json;
using RestSharp;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Checkout.ExternalClients
{
    public class ApiClientBase
    {
        private readonly string _baseUrl;

        protected ApiClientBase(string url)
        {
            _baseUrl = url;
        }

        protected async Task<T> Get<T>(RestClient restClient, string url)
        {
            var uri = BuildUri(url);
            var request = new RestRequest(uri);

            var response = await Execute<T>(restClient, request);
            return response.Data;
        }

        protected async Task<RestResponse<T>> Post<T>(RestClient restClient, string url, object payload)
        {
            var jsonBody = JsonConvert.SerializeObject(payload);
            var uri = BuildUri(url);

            var request = new RestRequest(uri, Method.Post);

            request.AddJsonBody(payload);            

            return await Execute<T>(restClient, request);
        }

        private async Task<RestResponse<T>> Execute<T>(RestClient restClient, RestRequest request)
        {
            RestResponse<T> restResponse;

            try
            {
                restResponse = await restClient.ExecuteAsync<T>(request);
            }
            catch (Exception e)
            {
                throw new ExternalServiceHttpException((int)HttpStatusCode.InternalServerError, e.Message);
            }

            ProcessResponse(restResponse);

            return restResponse;
        }

        private void ProcessResponse(RestResponse restResponse)
        {
            // if response status code is 404, the request is deemed successful, just exit and let it to return null to the caller 

            if (restResponse.IsSuccessful 
                || restResponse.StatusCode == HttpStatusCode.NotFound)
            {
                return;
            }

            if (restResponse.StatusCode == 0 && restResponse.ErrorException != null)
            {
                throw new ExternalServiceHttpException(0, restResponse.ErrorException.Message);
            }

            // if bad request, it might contain an important message to pass to the user
            // of course, in the real world we need carefuly evaluate what we can expose to the user
            if(restResponse.StatusCode == HttpStatusCode.BadRequest)
            {
                throw new ExternalServiceHttpException((int)restResponse.StatusCode, restResponse.Content);
            }

            throw new ExternalServiceHttpException((int)restResponse.StatusCode, restResponse.StatusDescription);
        }

        public Uri BuildUri(string url)
        {
            return string.IsNullOrEmpty(_baseUrl) ? new Uri(url) : new Uri($"{_baseUrl}/{url}");
        }
    }
}
