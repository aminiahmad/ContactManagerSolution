using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Rotativa.AspNetCore;
using Rotativa.AspNetCore.Options;
using ServiceContract;
using ServiceContract.DTO;
using ServiceContract.Enums;
using System.IO;
using System.Runtime.CompilerServices;
using CRUDExample.Filters;
using CRUDExample.Filters.ActionFilter;
using CRUDExample.Filters.AuthorizationFilters;
using CRUDExample.Filters.ExceptionFilter;
using CRUDExample.Filters.ResourseFilter;
using CRUDExample.Filters.ResultFilters;

namespace CRUDExample.Controllers
{
    //persons/Route of method
    //[Route("Persons")]
    //[controller]/[action]
    //[controller] name of controller == Persons

    [Route("[controller]")]
    //[TypeFilter(typeof(ResponseHeaderActionFilter),Arguments = new object[]{"my-key-fromController","my-value-fromController",3},Order=3)]
    [ResponseHeaderFilterFactory("my-Key-From-Controller","my-value-fromController",3)]
    //[TypeFilter(typeof(HandleExceptionFilter))]
    [TypeFilter(typeof(PersonAlwaysResultFilter))]
    public class PersonsController : Controller
    {
        private readonly IPersonGetterService _personsGetterService;
        private readonly IPersonAdderService _personsAdderService;
        private readonly IPersonSorterService _personsSorterService;
        private readonly IPersonDeleterService _personsDeleterService;
        private readonly IPersonUpdaterService _personsUpdaterService;
        private readonly ICountryGetterService _countryService;
        private readonly ILogger<PersonsController> _logger;
        //constructor
        public PersonsController(IPersonGetterService personsGetterService
            ,IPersonAdderService personsAdderService
            ,IPersonSorterService personsSorterService
            ,IPersonDeleterService personsDeleterService
            ,IPersonUpdaterService personsUpdaterService
            ,ICountryGetterService countryService, ILogger<PersonsController> logger)
        {
            _personsGetterService = personsGetterService;
            _personsAdderService = personsAdderService;
            _personsSorterService = personsSorterService;
            _personsDeleterService = personsDeleterService;
            _personsUpdaterService = personsUpdaterService;
            _countryService = countryService;
            _logger = logger;
        }


        [Route("/")]
        [Route("index")]

        //different between service filter with type filter is type filter supply argument but with service filter not supply argument
        [ServiceFilter(typeof(PersonsListActionFilter),Order = 4)]
        [ResponseHeaderFilterFactory("my-key-fromAction","my-value-fromAction",1)]
        [TypeFilter(typeof(PersonsListResultFilter))]
        [SkipFilter]
        public async Task<IActionResult> Index(string searchBy, string? searchString,string sortBy=nameof(PersonResponse.PersonName)
            ,SortOptions sortOptions=SortOptions.Asc )
        {
            _logger.LogInformation("index method of personsController");
            _logger.LogDebug($"searchBy:{searchBy} searchString:{searchString} sortBy:{searchBy} sortOptions:{sortOptions}");

            List<PersonResponse> persons = await _personsGetterService.GetFilteredPersons(searchBy, searchString);

            List<PersonResponse> sortedPersons= await _personsSorterService.GetSortedPersons(persons, sortBy, sortOptions);

            //ViewBag.CurrentSortBy=sortBy;
            //ViewBag.CurrentSortOption=sortOptions.ToString();

            return View(sortedPersons);
            
        }


        [Route("[action]")]
        [HttpGet]
        //[TypeFilter(typeof(ResponseHeaderActionFilter),Arguments = new object[]{"y-custom-key","y-custom-value",4})]
        [ResponseHeaderFilterFactory("y-custom-key","y-custom-value",4)]
        public async Task<IActionResult> Create()
        {
            List<CountryResponse> countries=await _countryService.GetAllCountry();
            ViewBag.Countries = countries.Select(temp =>
            
                new SelectListItem()
                {
                    Text = temp.CountryName,
                    Value = temp.CountryId.ToString()
                }
            ); 
            return View();
        }

        // Execute when user complete form register and click create link 

        [Route("[action]")]
        [HttpPost]
        [TypeFilter(typeof(PersonCreateAndEditActionFilter))]
        //resource filter
        [TypeFilter(typeof(FeatureDisableResourceFilter),Arguments = new object[]{false})]
        public async Task<IActionResult> Create(PersonAddRequest personRequest)
        {
            
            PersonResponse person =await _personsAdderService.AddPerson(personRequest);
            return RedirectToAction("Index","Persons");
        }

        [Route("[action]/{personId}")]
        [HttpGet]
        [TypeFilter(typeof(TokenResultFilter))]
        public async Task<IActionResult> Edit(Guid personId)
        {
            PersonResponse? personResponse=await _personsGetterService.GetPersonByPersonId(personId);
            if (personResponse == null)
            {
                RedirectToAction("Index");
            }

            PersonUpdateRequest? personUpdate= personResponse?.ToPersonUpdate();
            List<CountryResponse> countries= await _countryService.GetAllCountry();
            ViewBag.Countries = countries.Select(temp =>

                new SelectListItem()
                {
                    Text = temp.CountryName,
                    Value = temp.CountryId.ToString()
                });
            return View(personUpdate);
        }


        [Route("[action]/{personId}")]
        [HttpPost]
        [TypeFilter(typeof(PersonCreateAndEditActionFilter))]
        [TypeFilter(typeof(TokenAuthorizationFilter))]
        [TypeFilter(typeof(PersonAlwaysResultFilter))]
        public async Task<IActionResult> Edit(PersonUpdateRequest personRequest)
        {
            PersonResponse? personResponse=await _personsGetterService.GetPersonByPersonId(personRequest.PersonId);
            if (personResponse == null)
            {
                return RedirectToAction("Index");
            }
            //personRequest.PersonId = Guid.NewGuid();  Custom Exception practice
            PersonResponse updatePerson=await _personsUpdaterService.GetUpdatePerson(personRequest);
            return RedirectToAction("Index");
            
        }


        [HttpGet]
        [Route("[action]/{personId}")]
        public async Task<IActionResult> Delete(Guid personId)
        {
            PersonResponse? personResponse= await _personsGetterService.GetPersonByPersonId(personId);
            if (personResponse == null)
            {
                return RedirectToAction("Index");
            }

            return View(personResponse);
        }

        [HttpPost]
        [Route("[action]/{personId}")]
        public async Task<IActionResult> Delete(PersonUpdateRequest personUpdateResult)
        {
            if (personUpdateResult.PersonId == null)
            {
                return RedirectToAction("Index");
            }

            await _personsDeleterService.DeletePerson(personUpdateResult.PersonId);
            return RedirectToAction("Index");
        }

        [Route("[action]")]
        public async Task<IActionResult> PersonsPdf()
        {
            List<PersonResponse> persons= await _personsGetterService.GetAllPersons();

            return new ViewAsPdf("PersonsPdf", persons, ViewData)
            {
                PageMargins = new Margins(top:20,right:10,left:10,bottom:20),
                PageOrientation = Orientation.Landscape
            };
        }

        [Route("[action]")]
        public async Task<IActionResult> PersonsCsv()
        {
            MemoryStream memoryStream = await _personsGetterService.GetPersonsCsv();
            return File(memoryStream, "application/octet-stream", "persons.csv");
        }
        [Route("[action]")]
        public async Task<IActionResult> PersonsExcel()
        {
            MemoryStream memoryStream = await _personsGetterService.GetPersonsExcel();
            return File(memoryStream, "application/vnd.ms-excel", "persons.xlsx");
        }
    }
}
