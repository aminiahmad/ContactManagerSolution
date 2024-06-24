using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CRUDExample.Filters.ResourseFilter;

public class FeatureDisableResourceFilter:IAsyncResourceFilter
{
    private readonly ILogger<FeatureDisableResourceFilter> _logger;
    private readonly bool _isDisable;
    public FeatureDisableResourceFilter(ILogger<FeatureDisableResourceFilter> logger,bool isDisable=true)
    {
        _isDisable = isDisable;
        _logger= logger;
    }
    public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
    {
        _logger.LogInformation("{filterName}.{methodName} - before" 
            ,nameof(FeatureDisableResourceFilter),nameof(OnResourceExecutionAsync));
        if (_isDisable)
        {
            //context.Result = new NotFoundResult();//404 - not found

            context.Result = new StatusCodeResult(501);//501 - not implement
        }
        else
        {
           await next();
        }
        _logger.LogInformation("{filterName}.{methodName} - after" 
            ,nameof(FeatureDisableResourceFilter),nameof(OnResourceExecutionAsync));
    }
}