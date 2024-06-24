
using ServiceContract;
using ServiceContract.DTO;
using Entities;
using RepositoryContract;

namespace Service
{
    public class CountryAdderService : ICountryAdderService
    {
        private readonly ICountriesRepository _countriesRepository;

        public CountryAdderService(ICountriesRepository countriesRepository)
        {
            _countriesRepository = countriesRepository;
            
        }

        public async Task<CountryResponse> AddCountry(CountryAddRequest? request)
        {
            // error handling (for testing)
            // object add request is null
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }
            // countryName of addRequest is null
            if (request.CountryName == null)
            {
                throw new ArgumentException(nameof(request));
            }
            // Duplicate CountryName 
            if (await _countriesRepository.GetCountryByName(request.CountryName)!=null)
            {
                throw new ArgumentException("Given country name already exists");
            }

            //initial value
            Country country = request.ToCountry();
            country.CountryId=Guid.NewGuid();

            //add to list of country
            await _countriesRepository.AddCountry(country);

            return country.ToCountryResponse();
        }

    }
}
