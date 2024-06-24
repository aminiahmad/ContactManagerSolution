using ServiceContract.DTO;

namespace ServiceContract
{
    public interface ICountryAdderService
    {
        /// <summary>
        /// add country object to the list of countries
        /// </summary>
        /// <param name="request">country object to add</param>
        /// <returns>return the country object after adding it</returns>
        Task<CountryResponse> AddCountry(CountryAddRequest?  request);

    }
}
