using CRUDExample.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using ServiceContract;
using ServiceContract.DTO;

namespace CRUDExample.Filters.ActionFilter;

public class PersonCreateAndEditActionFilter:IAsyncActionFilter
{
    private readonly ICountryGetterService _countryService;
    private readonly ILogger<PersonCreateAndEditActionFilter> _logger;
    public PersonCreateAndEditActionFilter(ICountryGetterService countryService, ILogger<PersonCreateAndEditActionFilter> logger)
    {

        _countryService = countryService;
        _logger = logger;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (context.Controller is PersonsController personsController)
        {
            if (!personsController.ModelState.IsValid)
            {
                List<CountryResponse> countries = await _countryService.GetAllCountry();
                personsController.ViewBag.Countries = countries.Select(temp =>

                    new SelectListItem()
                    {
                        Text = temp.CountryName,
                        Value = temp.CountryId.ToString()
                    }
                );
                personsController.ViewBag.ErrorsCreatePerson = personsController.ModelState.Values.SelectMany(value => value.Errors)
                    .Select(e => e.ErrorMessage).ToList();
                var personRequest = context.ActionArguments["personRequest"];
                context.Result = personsController.View(personRequest);
            }
            else
            {
                await next();
            }
        }
        else
        {
            await next();
        }

        //short circuit execution
        //TO DO: After Logic
        _logger.LogInformation("In after logic of personCreateAndEdit ActionFilter");
    }
}