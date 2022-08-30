using System.Diagnostics;
using System.Web.Http.Filters;

namespace Api.Attributes
{
    public class GlobalExceptionHandlerFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext context)
        {
            Trace.TraceError(context.Exception.ToString());
        }
    }
}