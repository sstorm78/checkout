using System;
using System.Net;

namespace Checkout.ExternalClients
{
    public class ExternalServiceHttpException : Exception
    {
        public HttpStatusCode StatusCode { get; }

        public ExternalServiceHttpException(int statusCode, string message)
            : base(message)
        {
            StatusCode = (HttpStatusCode)statusCode;
        }
    }
}
