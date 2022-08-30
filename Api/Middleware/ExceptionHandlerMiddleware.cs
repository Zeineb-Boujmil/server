using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using CED.Framework.Logging;
using Microsoft.Owin;

namespace Api.Middleware
{
    using AppFunc = Func<IDictionary<string, object>, Task>;

    public class ExceptionHandlerMiddleware
    {
        private readonly AppFunc _next;
        private static readonly string LoggerName = ConfigurationManager.AppSettings["LoggerName"];
        private readonly Logger _logger;

        public ExceptionHandlerMiddleware(AppFunc next)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _logger = Logger.GetLogger(LoggerName);
        }

        public async Task Invoke(IDictionary<string, object> environment)
        {
            try
            {
                await _next(environment);
            }
            catch (Exception exception)
            {

                Trace.TraceError(exception.ToString());
                _logger.Error(exception.Message, exception, Guid.NewGuid());

                try
                {
                    var context = new OwinContext(environment);

                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.ReasonPhrase = "Internal Server Error";
                    context.Response.ContentType = "application/json";
                    context.Response.Write(exception.ToString());

                    return;
                }
                catch
                {
                    // If there's a Exception while generating the error page, re-throw the original exception.
                }

                throw;
            }
        }
    }
}