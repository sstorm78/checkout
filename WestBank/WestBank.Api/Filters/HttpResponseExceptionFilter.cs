using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WestBank.Api.Filters
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

            context.Result = new ObjectResult("An error has occured, please try again later")
                             {
                                 StatusCode = 503
                             };
                context.ExceptionHandled = true;
            }
    }
}
