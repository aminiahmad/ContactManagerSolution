using Entities;
using Xunit.Abstractions;
using Xunit;
using ServiceContract.DTO;
using ServiceContract.Enums;
using ServiceContract;
using Service;
using Microsoft.Extensions.Options;
using AutoFixture;
using FluentAssertions;
using System;
using System.Linq.Expressions;
using Microsoft.Extensions.Logging;
using Moq;
using RepositoryContract;

namespace CRUDTest;

public class PersonsServiceTest
{
    //field
    private readonly IPersonGetterService _personsGetterService;
    private readonly IPersonAdderService _personsAdderService;
    private readonly IPersonSorterService _personsSorterService;
    private readonly IPersonDeleterService _personsDeleterService;
    private readonly IPersonUpdaterService _personsUpdaterService;
    //private readonly ICountryService _countryService;
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly IFixture _fixture;

    private readonly IPersonsRepository _personsRepository;
    private readonly Mock<ILogger<PersonsGetterService>> _loggerGetterMock;
    private readonly Mock<ILogger<PersonsAdderService>> _loggerAdderMock;
    private readonly Mock<ILogger<PersonsSorterService>> _loggerSorterMock;
    private readonly Mock<ILogger<PersonsDeleterService>> _loggerDeleterMock;
    private readonly Mock<ILogger<PersonsUpdaterService>> _loggerUpdaterMock;
    private readonly Mock<IPersonsRepository> _personsRepositoryMock;

    //constructor
    public PersonsServiceTest(ITestOutputHelper testOutputHelper)
    {
        _fixture=new Fixture();
        //var initialCountries= new List<Country>(){};
        //var initialPersons= new List<Person>(){};
        //DbContextMock<ApplicationDbContext> dbContextMock = new DbContextMock<ApplicationDbContext>
            //(new DbContextOptionsBuilder<ApplicationDbContext>().Options);

        //ApplicationDbContext dbContext= dbContextMock.Object;

        //dbContextMock.CreateDbSetMock(temp => temp.Countries, initialCountries);
        //dbContextMock.CreateDbSetMock(temp => temp.Persons, initialPersons);

        _personsRepositoryMock= new Mock<IPersonsRepository>();
        _loggerGetterMock = new Mock<ILogger<PersonsGetterService>>();
        _loggerAdderMock = new Mock<ILogger<PersonsAdderService>>();
        _loggerSorterMock = new Mock<ILogger<PersonsSorterService>>();
        _loggerDeleterMock = new Mock<ILogger<PersonsDeleterService>>();
        _loggerUpdaterMock = new Mock<ILogger<PersonsUpdaterService>>();
        _personsRepository= _personsRepositoryMock.Object;
        _personsGetterService = new PersonsGetterService(_personsRepository,_loggerGetterMock.Object);
        _personsAdderService = new PersonsAdderService(_personsRepository,_loggerAdderMock.Object);
        _personsSorterService = new PersonsSorterService(_personsRepository,_loggerSorterMock.Object);
        _personsDeleterService = new PersonsDeleterService(_personsRepository,_loggerDeleterMock.Object);
        _personsUpdaterService = new PersonsUpdaterService(_personsRepository,_loggerUpdaterMock.Object);

        //_countryService = new CountryService(null);
        //_countryService = new CountryService(new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>().Options));
        //_personsGetterService = new PersonsService(new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>().Options),_countryService);
        _testOutputHelper = testOutputHelper;
    }



    //test cases
    #region AddPerson

    [Fact]
    public async Task AddPerson_NullPerson_ToBeArgumentNullException()
    {
        //arrange
        PersonAddRequest? request = null;

        //assert
        /*await Assert.ThrowsAsync<ArgumentNullException>(async() =>
        {
            //act
           await _personsGetterService.AddPerson(request);
        });*/

        //act
        Func<Task> action = async () =>
        {
            
            await _personsAdderService.AddPerson(request);
        };
        //assert
        await action.Should().ThrowAsync<ArgumentNullException>();

    }
    [Fact]
    public async Task AddPerson_NullPersonName_ToBeArgumentException()
    {
        //arrange
        PersonAddRequest personAddRequest = _fixture.Build<PersonAddRequest>()
            .With(temp => temp.PersonName, null as string)
            .With(temp=>temp.Email,"samwdk@gmail.com")
            .Create();
        Person person= personAddRequest.ToPerson();
        
        //when personsRepository.addPerson is called, it has to return same "person" object
        _personsRepositoryMock.Setup(temp => temp.AddPerson(It.IsAny<Person>()))
            .ReturnsAsync(person);

        //act
        Func<Task> action = async () =>
        {
            
            await _personsAdderService.AddPerson(personAddRequest);
        };
        //assert
        await action.Should().ThrowAsync<ArgumentException>();
    }
    [Fact]
    public async Task AddPerson_FullPersonDetails_ToBeSuccessFul()
    {
        //arrange
        PersonAddRequest request = _fixture.Build<PersonAddRequest>().With(temp => temp.Email
            , "sample@gmail.com").Create();
        Person person = request.ToPerson();
        PersonResponse personResponse_expected=person.ToPersonResponse();

        _personsRepositoryMock.Setup(temp => temp.AddPerson(It.IsAny<Person>()))
            .ReturnsAsync(person);
        //act
        PersonResponse response= await _personsAdderService.AddPerson(request);
        personResponse_expected.PersonId = response.PersonId;
        //List<PersonResponse> listOfPersons = await _personsGetterService.GetAllPersons();
        //assert
        //Assert.True(response.PersonId!=Guid.Empty);
        response.PersonId.Should().NotBe(Guid.Empty);
        response.Should().Be(personResponse_expected);
        //Assert.Contains(response, listOfPersons);
        //listOfPersons.Should().Contain(response);
    }
    #endregion

    #region GetPersonByPersonId

    //if we supply null as person id should return null PersonResponse 
    [Fact]
    public async Task GetPersonByPersonId_NullPersonId_ToBeNull()
    {
        //arrange
        Guid? personId= null;
        //act
        PersonResponse? response =await _personsGetterService.GetPersonByPersonId(personId);
        //assert
        //Assert.Null(response);
        response.Should().BeNull();
    }

    //if we supply valid person id should return valid person detail as PersonResponse object
    [Fact]
    public async Task GetPersonByPersonId_WithPersonId_ToBeSuccessFul()
    {
        //arrange
        Person person = _fixture.Build<Person>()
            .With(temp => temp.Email, "sample7@gmail.com")
            .With(temp => temp.Country,null as Country)
            .Create();
        PersonResponse personResponseExcepted = person.ToPersonResponse();
        _personsRepositoryMock.Setup(temp => temp.GetPersonByPersonId(It.IsAny<Guid>()))
            .ReturnsAsync(person);
        //act
        PersonResponse? personGetResponse = await _personsGetterService.GetPersonByPersonId(person.PersonId);

        //assert
        //Assert.Equal(personAddResponse,personGetResponse);
        personGetResponse.Should().Be(personResponseExcepted);
    }
    #endregion

    #region GetAllPersons

    // list of persons should be first empty list
    [Fact]
    public async Task GetAllPersons_ToBeEmptyList()
    {
        List<Person> personList=new List<Person>();
        _personsRepositoryMock.Setup(temp => temp.GetAllPersons())
            .ReturnsAsync(personList);


        List<PersonResponse> responses = await _personsGetterService.GetAllPersons();
        //Assert.Empty(responses);
        responses.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllPersons_AddFewPersons_ToBeSuccessFul()
    {
        //arrange
        List<Person> persons=new List<Person>()
        {
            _fixture.Build<Person>()
                .With(temp => temp.Email, "sample@gmail.com")
                .With(temp => temp.PersonName, "mahdi")
                .With(temp=>temp.Country,null as Country)
                .Create(),
            _fixture.Build<Person>()
                .With(temp => temp.Email, "sample2@gmail.com")
                .With(temp => temp.PersonName, "javad")
                .With(temp=>temp.Country,null as Country)
                .Create(),
            _fixture.Build<Person>()
                .With(temp => temp.Email, "sample3@gmail.com")
                .With(temp => temp.PersonName, "rahman")
                .With(temp=>temp.Country,null as Country)
                .Create(),
        };
        List<PersonResponse> personResponseFromExpected = persons.Select(temp => temp.ToPersonResponse()).ToList();
        //expected
        _testOutputHelper.WriteLine("Expected");
        foreach (var expectPerson in personResponseFromExpected)
        {
            _testOutputHelper.WriteLine(expectPerson.ToString());
        }

        _personsRepositoryMock.Setup(temp => temp.GetAllPersons())
            .ReturnsAsync(persons);

        //act
        List<PersonResponse> listOfActualPersons=await _personsGetterService.GetAllPersons();
        //actual
        _testOutputHelper.WriteLine("Actual");
        foreach (var actualPerson in listOfActualPersons)
        {
            _testOutputHelper.WriteLine(actualPerson.ToString());
        }

        
        //assert
        /*foreach (var expectedResponse in listOfPersonResponses)
        {
            Assert.Contains(expectedResponse, listOfActualPersons);
        }*/
        
        //assert
        listOfActualPersons.Should().BeEquivalentTo(personResponseFromExpected);

    }
    #endregion

    #region GetFilteredPersons

    //search empty text for person name field and return all persons object  
    [Fact]
    public async Task GetFilteredPersons_EmptySearchText_ToBeSuccessFul()
    {
        //arrange
        List<Person> persons=new List<Person>()
        {
            _fixture.Build<Person>()
                .With(temp => temp.Email, "sample@gmail.com")
                .With(temp => temp.PersonName, "mahdi")
                .With(temp=>temp.Country,null as Country)
                .Create(),
            _fixture.Build<Person>()
                .With(temp => temp.Email, "sample2@gmail.com")
                .With(temp => temp.PersonName, "javad")
                .With(temp=>temp.Country,null as Country)
                .Create(),
            _fixture.Build<Person>()
                .With(temp => temp.Email, "sample3@gmail.com")
                .With(temp => temp.PersonName, "rahman")
                .With(temp=>temp.Country,null as Country)
                .Create(),
        };

        List<PersonResponse> listOfPersonExpected = persons.Select(temp => temp.ToPersonResponse()).ToList();    
        //expected
        _testOutputHelper.WriteLine("Expected");
        foreach (var expectPerson in listOfPersonExpected)
        {
            _testOutputHelper.WriteLine(expectPerson.ToString());
        }

        _personsRepositoryMock.Setup(temp => temp.GetFilteredPersons(It.IsAny<Expression<Func<Person,bool>>>()))
            .ReturnsAsync(persons);
        //act
        List<PersonResponse> listOfActualPersons= await _personsGetterService.GetFilteredPersons(nameof(Person.PersonName), "");
        //actual
        _testOutputHelper.WriteLine("Actual");
        foreach (var actualPerson in listOfActualPersons)
        {
            _testOutputHelper.WriteLine(actualPerson.ToString());
        }

        //assert
        listOfActualPersons.Should().BeEquivalentTo(listOfPersonExpected);

    }

    //search part of text or full text for person name field and return matching persons object  
    [Fact]
    public async Task GetFilteredPersons_SearchByPersonName_ToBeSuccessFul()
    {
        //arrange
        List<Person> persons=new List<Person>()
        {
            _fixture.Build<Person>()
                .With(temp => temp.Email, "sample@gmail.com")
                .With(temp => temp.PersonName, "mahdi")
                .With(temp=>temp.Country,null as Country)
                .Create(),
            _fixture.Build<Person>()
                .With(temp => temp.Email, "sample2@gmail.com")
                .With(temp => temp.PersonName, "javad")
                .With(temp=>temp.Country,null as Country)
                .Create(),
            _fixture.Build<Person>()
                .With(temp => temp.Email, "sample3@gmail.com")
                .With(temp => temp.PersonName, "rahman")
                .With(temp=>temp.Country,null as Country)
                .Create(),
        };

        List<PersonResponse> listOfPersonExpected = persons.Select(temp => temp.ToPersonResponse()).ToList();    
        //expected
        _testOutputHelper.WriteLine("Expected");
        foreach (var expectPerson in listOfPersonExpected)
        {
            _testOutputHelper.WriteLine(expectPerson.ToString());
        }

        _personsRepositoryMock.Setup(temp => temp.GetFilteredPersons(It.IsAny<Expression<Func<Person,bool>>>()))
            .ReturnsAsync(persons);
        //act
        List<PersonResponse> listOfActualPersons= await _personsGetterService.GetFilteredPersons(nameof(Person.PersonName), "ma");
        //actual
        _testOutputHelper.WriteLine("Actual");
        foreach (var actualPerson in listOfActualPersons)
        {
            _testOutputHelper.WriteLine(actualPerson.ToString());
        }
        //assert
        /*foreach (var expectedResponse in listOfPersonResponses)
        {
            if (expectedResponse.PersonName != null)
            {
                if (expectedResponse.PersonName.Contains("ma", StringComparison.OrdinalIgnoreCase))
                {
                    Assert.Contains(expectedResponse, listOfActualPersons);
                }
            }
        }*/
        //assert
        listOfActualPersons.Should().BeEquivalentTo(listOfPersonExpected);

    }
    #endregion

    #region GetSortedPersons

    [Fact]
    public async Task GetSortedPersons_ToBeSuccessFul()
    {
        //arrange
        List<Person> persons=new List<Person>()
        {
            _fixture.Build<Person>()
                .With(temp => temp.Email, "sample@gmail.com")
                .With(temp => temp.PersonName, "mahdi")
                .With(temp=>temp.Country,null as Country)
                .Create(),
            _fixture.Build<Person>()
                .With(temp => temp.Email, "sample2@gmail.com")
                .With(temp => temp.PersonName, "javad")
                .With(temp=>temp.Country,null as Country)
                .Create(),
            _fixture.Build<Person>()
                .With(temp => temp.Email, "sample3@gmail.com")
                .With(temp => temp.PersonName, "rahman")
                .With(temp=>temp.Country,null as Country)
                .Create(),
        };

        List<PersonResponse> listOfPersonExpected = persons.Select(temp => temp.ToPersonResponse()).ToList();    
        //expected
        _testOutputHelper.WriteLine("Expected");
        foreach (var expectPerson in listOfPersonExpected)
        {
            _testOutputHelper.WriteLine(expectPerson.ToString());
        }
        //act
        _personsRepositoryMock
            .Setup(temp => temp.GetAllPersons())
            .ReturnsAsync(persons);
        List<PersonResponse> allPersons=await _personsGetterService.GetAllPersons();
        List<PersonResponse> listOfActualPersons=await _personsSorterService.GetSortedPersons(allPersons,nameof(Person.PersonName)
            ,SortOptions.Desc);

        //actual
        _testOutputHelper.WriteLine("Actual");
        foreach (var actualPerson in listOfActualPersons)
        {
            _testOutputHelper.WriteLine(actualPerson.ToString());
        }

        //complete expected
        //listOfPersonResponses= listOfPersonResponses.OrderByDescending(temp => temp.PersonName).ToList();

        //assert
        /*for (var i = 0; i < listOfPersonResponses.Count; i++)
        {
            Assert.Equal(listOfPersonResponses[i],listOfActualPersons[i]);
        }*/

        //assert
        listOfActualPersons.Should().BeInDescendingOrder(temp=>temp.PersonName);

    }

    #endregion

    #region GetUpdatePerson

    // supply null personUpdateRequest and return ArgumentNullException
    [Fact]
    public async Task GetUpdatePerson_NullPerson_ToBeArgumentNullException()
    {
        //arrange
        PersonUpdateRequest? personUpdateRequest = null;

        //assert
        /*await Assert.ThrowsAsync<ArgumentNullException>(async () =>
        {
            //act
            await _personsGetterService.GetUpdatePerson(personUpdateRequest);
        });*/

        //act
        var action=async () =>
        {
            
            await _personsUpdaterService.GetUpdatePerson(personUpdateRequest);
        };
        //assert
        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    // supply new personId and return ArgumentException
    [Fact]
    public async Task GetUpdatePerson_InvalidPersonID_ToBeArgumentException()
    {
        //arrange
        var personUpdateRequest =_fixture.Create<PersonUpdateRequest>();

        //assert
        /*await Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            //act
            await _personsGetterService.GetUpdatePerson(personUpdateRequest);
        });*/

        var action = async () =>
        {
            //act
            await _personsUpdaterService.GetUpdatePerson(personUpdateRequest);
        };
        //assert
        await action.Should().ThrowAsync<ArgumentException>();

    }

    // supply person name null value and return argumentException
    [Fact]
    public async Task GetUpdatePerson_PersonNameIsNull_ToBeArgumentException()
    {
        //arrange
        var person = _fixture.Build<Person>()
            .With(temp=>temp.PersonName,null as string)
            .With(temp => temp.Email, "sampl2@gamil.com")
            .With(temp => temp.Country, null as Country)
            .With(temp => temp.Gender, "Male")
            .Create();
        PersonResponse personResponse = person.ToPersonResponse();

        //act
        var personUpdateRequest = personResponse.ToPersonUpdate();
        personUpdateRequest.PersonName = null;

        //assert
        /*await Assert.ThrowsAsync<ArgumentException>(async() =>
        {
            await _personsGetterService.GetUpdatePerson(personUpdateRequest);
        });*/

        //act
        var action=async() =>
        {
            await _personsUpdaterService.GetUpdatePerson(personUpdateRequest);
        };
        //assert
        await action.Should().ThrowAsync<ArgumentException>();
    }

    //supply complete detail person object and check equal updated person and person object based on person id and validation change object
    [Fact]
    public async Task GetUpdatePerson_PersonFullDetailUpdate_ToBeSuccessFul()
    {
        //arrange
        var person = _fixture.Build<Person>()
            .With(temp => temp.Gender, "Male")
            .With(temp => temp.Country,null as Country)
            .With(temp => temp.Email, "sampl@gamil.com")
            .Create();
        PersonResponse personResponse = person.ToPersonResponse();

        var personUpdateRequest = personResponse.ToPersonUpdate();
        _personsRepositoryMock
            .Setup(temp => temp.GetPersonByPersonId(It.IsAny<Guid>()))
            .ReturnsAsync(person);
        _personsRepositoryMock
            .Setup(temp => temp.UpdatePerson(It.IsAny<Person>()))
            .ReturnsAsync(person);

        //act
        
        PersonResponse personResponseAdd= await _personsUpdaterService.GetUpdatePerson(personUpdateRequest);
        PersonResponse? personResponseGet = await _personsGetterService.GetPersonByPersonId(personResponseAdd.PersonId);

        //assert
        //Assert.Equal(personResponseAdd,personResponseGet);
        personResponseAdd.Should().Be(personResponseGet);
    }
    #endregion

    #region DeletePerson

    [Fact]
    public async Task DeletePerson_ValidPersonId_ToBeSuccessFul()
    {
        var person = _fixture.Build<Person>()
            .With(temp => temp.Country, null as Country)
            .With(temp => temp.Gender, "Male")
            .With(temp => temp.Email,"ashda@gmail.com")
            .Create();
        PersonResponse personResponse = person.ToPersonResponse();

        //expect
        _testOutputHelper.WriteLine("Expected : True");

        _personsRepositoryMock
            .Setup(temp => temp.DeletePersonByPersonId(It.IsAny<Guid>()))
            .ReturnsAsync(true);
        _personsRepositoryMock
            .Setup(temp => temp.GetPersonByPersonId(It.IsAny<Guid>()))
            .ReturnsAsync(person);

        //act
        bool isValid = await _personsDeleterService.DeletePerson(personResponse.PersonId);

        //Actual
        _testOutputHelper.WriteLine($"Actual {isValid}");
        //assert
        //Assert.True(isValid);
        isValid.Should().BeTrue();
    }
    [Fact]
    public async Task DeletePerson_InValidPersonId()
    {
        //expect
        _testOutputHelper.WriteLine("Expected : False");
        //act
        bool isValid = await _personsDeleterService.DeletePerson(Guid.NewGuid());
        _testOutputHelper.WriteLine($"Actual {isValid}");
        //assert
        //Assert.False(isValid);

        isValid.Should().BeFalse();
    }

    #endregion
}