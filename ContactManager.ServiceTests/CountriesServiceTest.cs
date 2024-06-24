using Entities;
using Moq;
using RepositoryContract;
using ServiceContract;
using ServiceContract.DTO;
using AutoFixture;
using Service;
using FluentAssertions;
namespace CRUDTest
{
    public class CountriesServiceTest
    {
        private readonly ICountryGetterService _countryGetterService;
        private readonly ICountryUploaderService _countryUploaderService;
        private readonly ICountryAdderService _countryAdderService;
        private readonly ICountriesRepository _countriesRepository;
        private readonly Mock<ICountriesRepository> _countriesRepositoryMock;
        private readonly IFixture _fixture;

        public CountriesServiceTest()
        {
            /*var initialCountries= new List<Country>(){};
            DbContextMock<ApplicationDbContext> dbContextMock = new DbContextMock<ApplicationDbContext>
                (new DbContextOptionsBuilder<ApplicationDbContext>().Options);*/

            /*ApplicationDbContext dbContext= dbContextMock.Object;
            dbContextMock.CreateDbSetMock(temp => temp.Countries, initialCountries);*/

            _fixture = new Fixture();

            _countriesRepositoryMock= new Mock<ICountriesRepository>();
            _countriesRepository = _countriesRepositoryMock.Object;
            _countryGetterService = new CountryGetterService(_countriesRepository);
            _countryUploaderService = new CountryUploaderService(_countriesRepository);
            _countryAdderService = new CountryAdderService(_countriesRepository);

            //_countryGetterService = new CountryService(new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>().Options));
        }


        #region AddCountry
        // add null object req country
        [Fact]
        public async Task AddCountry_NullCountry_ToBeArgumentNullException()
        {
            //Arrange
            CountryAddRequest? request = null;
            Country country = _fixture.Build<Country>()
                .With(temp => temp.Persons, null as List<Person>)
                .Create();
            _countriesRepositoryMock
                .Setup(temp => temp.AddCountry(It.IsAny<Country>()))
                .ReturnsAsync(country);
            //assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                //fact
                await _countryAdderService.AddCountry(request);
            });

        }
        // add countryName null in add req country
        [Fact]
        public async Task AddCountry_CountryNameIsNull_ToBeArgumentException()
        {
            //Arrange
            CountryAddRequest request = _fixture.Build<CountryAddRequest>()
                .With(temp => temp.CountryName, null as string)
                .Create();
            Country country = _fixture.Build<Country>()
                .With(temp => temp.Persons, null as List<Person>)
                .Create();
            _countriesRepositoryMock
                .Setup(temp => temp.AddCountry(It.IsAny<Country>()))
                .ReturnsAsync(country);
            //assert
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                //fact
                await _countryAdderService.AddCountry(request);
            });

        }
        // add Duplicate name in object  addReqCountry
        [Fact]
        public async Task AddCountry_DuplicateNameCountry_ToBeArgumentException()
        {
            //Arrange
            CountryAddRequest request1= _fixture.Build<CountryAddRequest>()
                .With(temp => temp.CountryName, "usa")
                .Create();
            CountryAddRequest request2= _fixture.Build<CountryAddRequest>()
                .With(temp => temp.CountryName, "usa")
                .Create();
            Country country1= request1.ToCountry();
            Country country2= request2.ToCountry();
            
            _countriesRepositoryMock
                .Setup(temp => temp.GetCountryByName(It.IsAny<string>()))
                .ReturnsAsync(null as Country);
            _countriesRepositoryMock
                .Setup(temp => temp.AddCountry(It.IsAny<Country>()))
                .ReturnsAsync(country1);
            CountryResponse countryResponse= await _countryAdderService.AddCountry(request1);

            //assert
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                //fact
                _countriesRepositoryMock
                    .Setup(temp => temp.GetCountryByName(It.IsAny<string>()))
                    .ReturnsAsync(country1);
                _countriesRepositoryMock
                    .Setup(temp => temp.AddCountry(It.IsAny<Country>()))
                    .ReturnsAsync(country1);
                await _countryAdderService.AddCountry(request2);
            });

        }
        //when you supply proper country name , it should insert(add)the country to the existing list of countries
        [Fact]
        public async Task AddCountry_ProperCountryDetail_ToBeSuccessFul()
        {
            //Arrange
            CountryAddRequest request1 = _fixture.Build<CountryAddRequest>()
                .With(temp => temp.CountryName, "usa")
                .Create();
            
            Country country= request1.ToCountry();
            CountryResponse countryResponse= country.ToCountryResponse();
            _countriesRepositoryMock
                .Setup(temp=>temp.AddCountry(It.IsAny<Country>()))
                .ReturnsAsync(country);
            _countriesRepositoryMock
                .Setup(temp=>temp.GetCountryByName(It.IsAny<string>()))
                .ReturnsAsync(null as Country);
            //fact
            CountryResponse response= await _countryAdderService.AddCountry(request1);

            country.CountryId= response.CountryId;
            countryResponse.CountryId= response.CountryId;

            //assert
            //Assert.True(response.CountryId!=Guid.Empty);
            response.CountryId.Should().NotBe(Guid.Empty);
            response.Should().BeEquivalentTo(countryResponse);

        }
        #endregion

        #region GetAllCountry

        [Fact]
        public async Task GetAllCountry_EmptyList_ToBeSuccessFul()
        {
            List<Country> country = new List<Country>();
            _countriesRepositoryMock
                .Setup(temp => temp.GetAllCountries())
                .ReturnsAsync(country);
            //act
            List<CountryResponse> responses= await _countryGetterService.GetAllCountry();
            //assert
            //Assert.Empty(responses);
            responses.Should().BeEmpty();
        }
        [Fact]
        public async Task GetAllCountry_AddFewCountries_ToBeSuccessFul()
        {

            //arrange
            List<Country> listOfInputCountries = new List<Country>()
                { _fixture.Build<Country>().With(temp=>temp.Persons,null as List<Person>).Create(),
                    _fixture.Build<Country>().With(temp=>temp.Persons,null as List<Person>).Create()
                    ,_fixture.Build<Country>().With(temp=>temp.Persons,null as List<Person>).Create()
                };
            List<CountryResponse> countryResponses= listOfInputCountries.Select(temp => temp.ToCountryResponse()).ToList();

            _countriesRepositoryMock
                .Setup(temp => temp.GetAllCountries())
                .ReturnsAsync(listOfInputCountries);

            List<CountryResponse> actualListOfCountry= await _countryGetterService.GetAllCountry();
            actualListOfCountry.Should().BeEquivalentTo(countryResponses);
        }

        #endregion

        #region GetCountryByCountryId

        [Fact]

        public async Task GetCountryByCountryId_NullCountryId()
        {
            //arrange
            Guid? countryId = null;
            _countriesRepositoryMock
                .Setup(temp => temp.GetCountryById(It.IsAny<Guid>()))
                .ReturnsAsync(null as Country);
            //act
            CountryResponse? countryResponseFromGetMethod= await _countryGetterService.GetCountryByCountryId(countryId);
            //assert
            //Assert.Null(countryResponseFromGetMethod);
            countryResponseFromGetMethod.Should().BeNull();
        }
        [Fact]
        public async Task GetCountryByCountryId_ValidCountryId_ToBeSuccessFul()
        {
            //arrange
            Country country = _fixture.Build<Country>().With(temp => temp.Persons, null as List<Person>).Create();
            _countriesRepositoryMock
                .Setup(temp => temp.GetCountryById(It.IsAny<Guid>()))
                .ReturnsAsync(country);
            _countriesRepositoryMock
                .Setup(temp => temp.AddCountry(It.IsAny<Country>()))
                .ReturnsAsync(country);

            CountryResponse responseFromAddCountry = country.ToCountryResponse();
            //act
            CountryResponse? responseFromGetCountry= await _countryGetterService.GetCountryByCountryId(responseFromAddCountry.CountryId);
            //assert
            //Assert.Equal(responseFromAddCountry,responseFromGetCountry);
            responseFromGetCountry.Should().BeEquivalentTo(responseFromAddCountry);
        }
        #endregion

    }
}
