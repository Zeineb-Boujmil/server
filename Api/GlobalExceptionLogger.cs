using CED.Framework.Logging;
using System;
using System.Configuration;
using System.Web.Http.ExceptionHandling;

namespace Api
{
    public class GlobalExceptionLogger: ExceptionLogger
    {
        private static readonly string LoggerName = ConfigurationManager.AppSettings["LoggerName"];
        private readonly Logger _logger;

        public GlobalExceptionLogger()
        {
            _logger = Logger.GetLogger(LoggerName);
        }

        public override void Log(ExceptionLoggerContext context)
        {
            _logger.Error(context.Exception.Message, context.Exception, Guid.NewGuid());
            base.Log(context);
        }
    }
}