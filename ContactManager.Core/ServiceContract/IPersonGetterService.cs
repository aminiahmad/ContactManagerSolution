using ServiceContract.DTO;

namespace ServiceContract;

/// <summary>
/// Represent Business Logic for manipulation person entity
/// </summary>
public interface IPersonGetterService
{
    
    /// <summary>
    /// return all persons
    /// </summary>
    /// <returns>return list of object of persons  </returns>
    Task<List<PersonResponse>> GetAllPersons();

    /// <summary>
    ///  return the person object based on the given person id
    /// </summary>
    /// <param name="personId">person id to search</param>
    /// <returns>return matching person object</returns>
    Task<PersonResponse?> GetPersonByPersonId(Guid? personId);


    /// <summary>
    /// return all matching person object with given filter by searchBy(field) and search(text for search)
    /// </summary>
    /// <param name="searchBy">search field for search</param>
    /// <param name="search"> search string for search</param>
    /// <returns>return all matching person object to list of personResponse </returns>
    Task<List<PersonResponse>> GetFilteredPersons(string searchBy, string? search);

    /// <summary>
    /// returns all person as csv file
    /// </summary>
    /// <returns>return memory data to csv</returns>
    Task<MemoryStream> GetPersonsCsv();
    /// <summary>
    /// returns all person as excel file
    /// </summary>
    /// <returns>return memory data to excel file</returns>
    Task<MemoryStream> GetPersonsExcel();
}