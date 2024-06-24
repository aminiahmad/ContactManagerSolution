using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Serilog;

namespace CRUDExample.Middleware
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class ExceptionHandleMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandleMiddleware> _logger;
        private readonly IDiagnosticContext _diagnosticContext;
        public ExceptionHandleMiddleware(RequestDelegate next , ILogger<ExceptionHandleMiddleware> logger, IDiagnosticContext diagnosticContext)
        {
            _next = next;
            _diagnosticContext = diagnosticContext;
            _logger = logger;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);

            }
            catch (Exception e)

            {
                if (e.InnerException != null)
                {
                    _logger.LogError("{ExceptionType} {ExceptionMessage}",e.InnerException.GetType().ToString()
                    ,e.InnerException.Message);
                }
                else
                {
                    _logger.LogError("{ExceptionType} {ExceptionMessage}",e.GetType().ToString()
                        ,e.Message);
                }

                //httpContext.Response.StatusCode = 500;
                //await httpContext.Response.WriteAsync("Error occurred");

                throw; // useExceptionHandler and redirect to /error
            }
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class ExceptionHandleMiddlewareExtensions
    {
        public static IApplicationBuilder UseExceptionHandleMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionHandleMiddleware>();
        }
    }
}
