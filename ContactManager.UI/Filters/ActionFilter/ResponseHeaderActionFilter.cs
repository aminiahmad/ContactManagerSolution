using Microsoft.AspNetCore.Mvc.Filters;

namespace CRUDExample.Filters.ActionFilter;

public class ResponseHeaderFilterFactory : Attribute, IFilterFactory
{
    private string Key { get; set; }
    private string Value { get; set; }
    private int Order { get; set; }
    public bool IsReusable =>false;

    public ResponseHeaderFilterFactory(string key,string value,int order)
    {
        Key = key;
        Value = value;
        Order = order;
    }
    public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
    {
        // controller -> filterFactory -> filter
       var filter= serviceProvider.GetRequiredService<ResponseHeaderActionFilter>();
       filter.Key = Key;
       filter.Value = Value;
       filter.Order = Order;
       return filter;
    }

}
public class ResponseHeaderActionFilter:IAsyncActionFilter,IOrderedFilter
{
    public string Key { get; set; }
    public string Value { get; set; }
    public int Order { get; set; }
    private readonly ILogger<ResponseHeaderActionFilter> _logger;
    public ResponseHeaderActionFilter(ILogger<ResponseHeaderActionFilter> logger)
    {
      _logger= logger;
    }
    
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        //before
        _logger.LogInformation("response header action filter before logic");
        context.HttpContext.Response.Headers[Key]=Value;

        await next();

        //after
        
        _logger.LogInformation("response header action filter after logic");
    }
}