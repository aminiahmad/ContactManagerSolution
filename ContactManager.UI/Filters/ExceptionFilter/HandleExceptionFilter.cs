using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CRUDExample.Filters.ExceptionFilter;

public class HandleExceptionFilter :IAsyncExceptionFilter
{
    private readonly ILogger<HandleExceptionFilter> _logger;
    private readonly IHostEnvironment _environment;

    public HandleExceptionFilter(ILogger<HandleExceptionFilter> logger, IHostEnvironment environment)
    {
        _logger = logger;
        _environment = environment;
    }

    public async Task OnExceptionAsync(ExceptionContext context)
    {
        _logger.LogError("Exception Filter {filterName}.{methodName} - {exceptionType} : {exceotionMessage}",
            nameof(HandleExceptionFilter),nameof(OnExceptionAsync),context.Exception.GetType().Name
            ,context.Exception.Message
            );
        if (_environment.IsDevelopment())
        {
            context.Result = new ContentResult()
            {
                Content = context.Exception.Message, ContentType = context.Exception.GetType().Name, StatusCode = 500
            };
        }

        return;
    }
}