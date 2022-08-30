using Api.Middleware;

// ReSharper disable once CheckNamespace
namespace Owin
{
    public static class AppBuilderExtensions
    {
        public static void UseExceptionHandler(this IAppBuilder app)
        {
            app.Use<ExceptionHandlerMiddleware>();
        }
    }
}