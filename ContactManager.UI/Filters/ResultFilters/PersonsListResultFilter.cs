using Microsoft.AspNetCore.Mvc.Filters;

namespace CRUDExample.Filters.ResultFilters;

public class PersonsListResultFilter:IAsyncResultFilter
{
    private readonly ILogger<PersonsListResultFilter> _logger;

    public PersonsListResultFilter(ILogger<PersonsListResultFilter> logger)
    {
        _logger= logger;
    }
    public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        _logger.LogInformation("{filterName}.{methodName} - before"
            ,nameof(PersonsListResultFilter),nameof(OnResultExecutionAsync));
        context.HttpContext.Response.Headers["Last-Modified"]=DateTime.Now.ToString("g");

        await next();

        _logger.LogInformation("{filterName}.{methodName} - before"
            ,nameof(PersonsListResultFilter),nameof(OnResultExecutionAsync));

    }
}