using ServiceContract.DTO;

namespace ServiceContract
{
    public interface ICountryGetterService
    {

        /// <summary>
        /// return all countries from the list
        /// </summary>
        /// <returns>all countries from the list as list of CountryResponse</returns>
        Task<List<CountryResponse>> GetAllCountry();
        /// <summary>
        /// return a country object based on the given countryId
        /// </summary>
        /// <param name="countryId">CountryId(Guid) to search</param>
        /// <returns>Matching country as countryResponse object</returns>
        Task<CountryResponse?> GetCountryByCountryId(Guid? countryId);


    }
}
