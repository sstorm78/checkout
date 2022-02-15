using Checkout.ExternalClients;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text.Json;

namespace Checkout.Api.Filters
{
    public class HttpResponseExceptionFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context) { }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Exception == null)
            {
                return;
            }

            if(context.Exception is ExternalServiceHttpException)
            {
                string message = string.Empty;

                if (!string.IsNullOrEmpty(context.Exception.Message))
                {
                    message = JsonSerializer.Deserialize<string>(context.Exception.Message);
                }

                context.Result = new ObjectResult(message)
                {
                    StatusCode = (int)((ExternalServiceHttpException)context.Exception).StatusCode
                };
                context.ExceptionHandled = true;
                return;
            }

            context.Result = new ObjectResult("An error has occured, please try again later")
                             {
                                 StatusCode = 503
                             };
                context.ExceptionHandled = true;
            }
    }
}
