
using ServiceContract;
using ServiceContract.DTO;
using Entities;
using RepositoryContract;

namespace Service
{
    public class CountryGetterService : ICountryGetterService
    {
        private readonly ICountriesRepository _countriesRepository;

        public CountryGetterService(ICountriesRepository countriesRepository)
        {
            _countriesRepository = countriesRepository;
            
        }

        public async Task<List<CountryResponse>> GetAllCountry()
        { 
           List<Country> countries= await _countriesRepository.GetAllCountries();
           return countries.Select(country => country.ToCountryResponse()).ToList();
        }

        public async Task<CountryResponse?> GetCountryByCountryId(Guid? countryId)
        {
            if (countryId == null) return null;

            Country? countryResponseList=await _countriesRepository.GetCountryById(countryId.Value);

            return countryResponseList?.ToCountryResponse();
        }

    }
}
