using System.Threading.Tasks;
using Entities;
namespace RepositoryContract

{
    /// <summary>
    /// represent data access logic for managing country entity
    /// </summary>
    public interface ICountriesRepository
    {
        /// <summary>
        /// return new country with new item
        /// </summary>
        /// <param name="country">country object to add </param>
        /// <returns>return country object with new country</returns>
        Task<Country> AddCountry(Country country);
        /// <summary>
        /// return list of country object
        /// </summary>
        /// <returns>return all countries</returns>
        Task<List<Country>> GetAllCountries();

        /// <summary>
        /// return country object with country id 
        /// </summary>
        /// <param name="countryId">country id to get matching object</param>
        /// <returns>return object of country based on country id (matching object or null return)</returns>
        Task<Country?> GetCountryById(Guid countryId);
        /// <summary>
        /// return country object based country name
        /// </summary>
        /// <param name="countryName">country name to get matching object</param>
        /// <returns></returns>
        Task<Country?> GetCountryByName(string countryName);

    }
}
