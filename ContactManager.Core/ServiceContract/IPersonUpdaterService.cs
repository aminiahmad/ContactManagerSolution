using ServiceContract.DTO;

namespace ServiceContract;

/// <summary>
/// Represent Business Logic for manipulation person entity
/// </summary>
public interface IPersonUpdaterService
{
    /// <summary>
    /// return all detail updated person object 
    /// </summary>
    /// <param name="personUpdateRequest">person to update</param>
    /// <returns>return the same person details, with before personId</returns>
    Task<PersonResponse> GetUpdatePerson(PersonUpdateRequest ? personUpdateRequest);

}