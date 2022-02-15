using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Checkout.ExternalServices.Tests.Tools
{
    public class StubRoute
    {
        public string HttpMethod { get; set; }

        public string Endpoint { get; set; }

        public TimeSpan Delay { get; set; }

        public HttpStatusCode StatusCode { get; set; }

        public string ResponseContent { get; set; }

        public List<KeyValuePair<string, StringValues>> Headers { get; set; }

        public Encoding ResponseEncoding { get; set; }

        public string ResponseContentType { get; set; }

        public Action<HttpContext> OnInvokedCallback { get; set; }

        public RequestDelegate RequestHandler
        {
            get
            {
                return async context =>
                {
                    if (Delay > TimeSpan.Zero)
                    {
                        await Task.Delay(Delay);
                    }

                    context.Response.StatusCode = (int)StatusCode;
                    context.Response.ContentType = ResponseContentType;

                    if(Headers != null)
                    {
                        foreach(var header in Headers)
                        {
                            context.Response.Headers.Add(header);
                        }
                    }

                    if (!string.IsNullOrEmpty(ResponseContent))
                    {
                        await context.Response.WriteAsync(ResponseContent, ResponseEncoding);
                    }

                    OnInvokedCallback?.Invoke(context);
                };
            }
        }
    }
}
