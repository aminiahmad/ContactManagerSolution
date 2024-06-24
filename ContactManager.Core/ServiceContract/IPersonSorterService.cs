using ServiceContract.DTO;
using ServiceContract.Enums;

namespace ServiceContract;

/// <summary>
/// Represent Business Logic for manipulation person entity
/// </summary>
public interface IPersonSorterService
{
    /// <summary>
    /// Return List Of Sorted Persons
    /// </summary>
    /// <param name="allPersons">List Of Persons For Sort</param>
    /// <param name="sortBy">Name of field based on list for sort</param>
    /// <param name="sortOption">Method Of Sort = ASC or DESC</param>
    /// <returns>Return Sorted List As List Of PersonResponse</returns>
    Task<List<PersonResponse>> GetSortedPersons(List<PersonResponse> allPersons, string sortBy, SortOptions sortOption);

}