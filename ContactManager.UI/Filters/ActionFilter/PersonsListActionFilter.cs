using CRUDExample.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using ServiceContract.DTO;
using ServiceContract.Enums;

namespace CRUDExample.Filters.ActionFilter;

public class PersonsListActionFilter(ILogger<PersonsListActionFilter> logger) : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (context.ActionArguments.ContainsKey("searchBy"))
        {
           string? searchBy= Convert.ToString(context.ActionArguments["searchBy"]);
           if (!string.IsNullOrEmpty(searchBy))
           {
                List<string> searchField= new List<string>()
                {
                    nameof(PersonResponse.PersonName),
                    nameof(PersonResponse.Email),
                    nameof(PersonResponse.Gender),
                    nameof(PersonResponse.CountryId),
                    nameof(PersonResponse.Address),
                    nameof(PersonResponse.DateOfBirth),
                };
                if (searchField.Any(temp => temp == searchBy) == false)
                {
                    logger.LogInformation("searchBy actual value {searchBy}",searchBy);
                    context.ActionArguments["searchBy"] = nameof(PersonResponse.PersonName);
                    logger.LogInformation("searchBy update value {searchBy}",context.ActionArguments["searchBy"]);
                }
           }
        }
        logger.LogInformation("{filterName}.{methodName} method"
            ,nameof(PersonsListActionFilter),nameof(OnActionExecuting));
        context.HttpContext.Items["Arguments"] = context.ActionArguments;
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {

        PersonsController personsController=(PersonsController)context.Controller;

        IDictionary<string,object?>? parameters=(Dictionary<string,object?>?) context.HttpContext.Items["Arguments"];

        if (parameters != null)
        {
            if (parameters.ContainsKey("searchBy"))
            {
                personsController.ViewData["CurrentSearchBy"] =Convert.ToString(parameters["searchBy"]);
            }
            if (parameters.ContainsKey("searchString"))
            {
                personsController.ViewData["CurrentSearchString"] =Convert.ToString(parameters["searchString"]);
            }
            if (parameters.ContainsKey("sortBy"))
            {
                personsController.ViewData["CurrentSortBy"] =Convert.ToString(parameters["sortBy"]);
            }
            else
            {
                personsController.ViewData["CurrentSortBy"] =nameof(PersonResponse.PersonName);

            }
            if (parameters.ContainsKey("sortOption"))
            {
                personsController.ViewData["CurrentSortOption"] =Convert.ToString(parameters["sortOption"]);
            }
            else
            {
                personsController.ViewData["CurrentSortOption"] = nameof(SortOptions.Asc);
            }
        }
        //search
        personsController.ViewBag.SearchFields = new Dictionary<string, string>()
        {
            {nameof(PersonResponse.PersonName),"Person Name"},
            {nameof(PersonResponse.Email),"Email"},
            {nameof(PersonResponse.Gender),"Gender"},
            {nameof(PersonResponse.DateOfBirth),"Date Of Birth"},
            {nameof(PersonResponse.Address),"Address"},
            {nameof(PersonResponse.Age),"Age"},
            {nameof(PersonResponse.Country),"Country"},
        };
        logger.LogInformation("{filterName}.{methodName} method"
            ,nameof(PersonsListActionFilter),nameof(OnActionExecuted));
    }
}