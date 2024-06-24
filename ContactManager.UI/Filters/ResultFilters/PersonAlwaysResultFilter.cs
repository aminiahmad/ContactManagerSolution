using Microsoft.AspNetCore.Mvc.Filters;

namespace CRUDExample.Filters.ResultFilters;

public class PersonAlwaysResultFilter:IAlwaysRunResultFilter
{
    public void OnResultExecuting(ResultExecutingContext context)
    {
        if (context.Filters.OfType<SkipFilter>().Any())
        {
            return;
        }
        //TO DO: Before
    }

    public void OnResultExecuted(ResultExecutedContext context)
    {
        
    }
}