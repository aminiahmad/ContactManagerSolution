using ServiceContract.DTO;

namespace ServiceContract;

/// <summary>
/// Represent Business Logic for manipulation person entity
/// </summary>
public interface IPersonAdderService
{
    /// <summary>
    /// add a new person into the list of persons
    /// </summary>
    /// <param name="personAddRequest">person to add</param>
    /// <returns>return the same person details, with generate new personId</returns>
    Task<PersonResponse> AddPerson(PersonAddRequest?  personAddRequest);
   
}