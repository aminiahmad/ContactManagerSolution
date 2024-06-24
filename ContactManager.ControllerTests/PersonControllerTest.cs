using AutoFixture;
using Moq;
using ServiceContract;
using FluentAssertions;
using CRUDExample.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ServiceContract.DTO;
using ServiceContract.Enums;

namespace CRUDTest;

public class PersonControllerTest
{
    private IFixture _fixture;
    private readonly ILogger<PersonsController> _logger;
    private readonly ICountryGetterService _countryService;
    private readonly IPersonGetterService _personsGetterService;
    private readonly IPersonAdderService _personsAdderService;
    private readonly IPersonSorterService _personsSorterService;
    private readonly IPersonDeleterService _personsDeleterService;
    private readonly IPersonUpdaterService _personsUpdaterService;
    private readonly Mock<IPersonGetterService> _personsGetterServiceMock;
    private readonly Mock<IPersonAdderService> _personsAdderServiceMock;
    private readonly Mock<IPersonSorterService> _personsSorterServiceMock;
    private readonly Mock<IPersonDeleterService> _personsDeleterServiceMock;
    private readonly Mock<IPersonUpdaterService> _personsUpdaterServiceMock;
    private readonly Mock<ICountryGetterService> _countryServiceMock;
    private readonly Mock<ILogger<PersonsController>> _loggerMock;
    public PersonControllerTest()
    {
        _fixture= new Fixture();
        _countryServiceMock=new Mock<ICountryGetterService>();
        _countryService = _countryServiceMock.Object;
        _loggerMock = new Mock<ILogger<PersonsController>>();
        _logger = _loggerMock.Object;
        _personsGetterServiceMock=new Mock<IPersonGetterService>();
        _personsGetterService = _personsGetterServiceMock.Object;
        _personsAdderServiceMock=new Mock<IPersonAdderService>();
        _personsAdderService = _personsAdderServiceMock.Object;
        _personsSorterServiceMock=new Mock<IPersonSorterService>();
        _personsSorterService = _personsSorterServiceMock.Object;
        _personsDeleterServiceMock=new Mock<IPersonDeleterService>();
        _personsDeleterService = _personsDeleterServiceMock.Object;
        _personsUpdaterServiceMock=new Mock<IPersonUpdaterService>();
        _personsUpdaterService = _personsUpdaterServiceMock.Object;
    }

    #region Index

    [Fact]
    public async Task Index_shouldReturnIndexView()
    {
        //arrange
        List<PersonResponse> personResponsesList = _fixture.Create<List<PersonResponse>>();
        PersonsController personsController = new PersonsController(_personsGetterService,_personsAdderService
            ,_personsSorterService,_personsDeleterService,_personsUpdaterService,_countryService,_logger);
        _personsGetterServiceMock
            .Setup(temp => temp.GetFilteredPersons(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(personResponsesList);
        _personsSorterServiceMock
            .Setup(temp => temp.GetSortedPersons(It.IsAny<List<PersonResponse>>()
                , It.IsAny<string>(), It.IsAny<SortOptions>()))
            .ReturnsAsync(personResponsesList);

        //act

        IActionResult result= await personsController.Index(_fixture.Create<string>(), _fixture.Create<string>()
            , _fixture.Create<string>(), _fixture.Create<SortOptions>());

        //assert

        ViewResult viewResult= Assert.IsType<ViewResult>(result);
        viewResult.ViewData.Model.Should().BeAssignableTo<IEnumerable<PersonResponse>>();
        viewResult.ViewData.Model.Should().Be(personResponsesList);
    }
    #endregion


    #region Create

    /*[Fact]
    public async void Create_IsModelError_ReturnCreateView()
    {
        //arrange
        PersonAddRequest personAddRequest = _fixture.Create<PersonAddRequest>();
        List<CountryResponse> countryResponsesList=_fixture.Create<List<CountryResponse>>();
        PersonResponse personResponse = _fixture.Create<PersonResponse>();
        PersonsController personsController = new PersonsController(_personsGetterService,_countryService, _logger);

        _personsGetterServiceMock
            .Setup(temp => temp.AddPerson(It.IsAny<PersonAddRequest>()))
            .ReturnsAsync(personResponse);
        _countryServiceMock
            .Setup(temp => temp.GetAllCountry())
            .ReturnsAsync(countryResponsesList);

        //act
        personsController.ModelState.AddModelError("PersonName","person name can't be blank");
        IActionResult result= await personsController.Create(personAddRequest);

        //assert

        ViewResult viewResult= Assert.IsType<ViewResult>(result);
        viewResult.ViewData.Model.Should().BeAssignableTo<PersonAddRequest>();
        viewResult.ViewData.Model.Should().Be(personAddRequest);
    }*/

    [Fact]
    public async void Create_IsNoModelError_ReturnRedirectToIndexView()
    {
        //arrange
        PersonAddRequest personAddRequest = _fixture.Create<PersonAddRequest>();
        List<CountryResponse> countryResponsesList=_fixture.Create<List<CountryResponse>>();
        PersonResponse personResponse = _fixture.Create<PersonResponse>();
        PersonsController personsController = new PersonsController(_personsGetterService,_personsAdderService
            ,_personsSorterService,_personsDeleterService,_personsUpdaterService,_countryService,_logger);
        _personsAdderServiceMock
            .Setup(temp => temp.AddPerson(It.IsAny<PersonAddRequest>()))
            .ReturnsAsync(personResponse);
        _countryServiceMock
            .Setup(temp => temp.GetAllCountry())
            .ReturnsAsync(countryResponsesList);

        //act
        IActionResult result= await personsController.Create(personAddRequest);

        //assert

        RedirectToActionResult redirectResult= Assert.IsType<RedirectToActionResult>(result);
        redirectResult.ActionName.Should().Be("Index");
    }
    #endregion
}