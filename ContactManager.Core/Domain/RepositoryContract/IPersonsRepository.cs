using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Entities;

namespace RepositoryContract;
/// <summary>
/// represent data access logic for managing person entity
/// </summary>
public interface IPersonsRepository
{

    /// <summary>
    /// return person object based on given person
    /// </summary>
    /// <param name="person">person object to add</param>
    /// <returns>return person object with new person</returns>
    Task<Person> AddPerson(Person person);

    /// <summary>
    /// return all person object items
    /// </summary>
    /// <returns>return all persons</returns>
    Task<List<Person>> GetAllPersons();

    /// <summary>
    /// return person detail based on person id (null or matching id) 
    /// </summary>
    /// <param name="personId">person id to get</param>
    /// <returns>return matching person</returns>
    Task<Person?> GetPersonByPersonId(Guid personId);

    /// <summary>
    /// return all persons based on the given expression
    /// </summary>
    /// <param name="predicate">LINQ expression to check</param>
    /// <returns>all matching persons with given expression</returns>
    Task<List<Person>> GetFilteredPersons(Expression<Func<Person, bool>> predicate);

    /// <summary>
    /// deletion person based on the given person id and return true if deletion else false
    /// </summary>
    /// <param name="personId">person id ro delete</param>
    /// <returns>return true if deletion is success</returns>
    Task<bool> DeletePersonByPersonId(Guid personId);

    /// <summary>
    /// update person based on given person object
    /// </summary>
    /// <param name="person">person obj to update</param>
    /// <returns>person object updated</returns>
    Task<Person> UpdatePerson(Person person);
}