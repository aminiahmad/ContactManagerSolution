namespace ServiceContract;

/// <summary>
/// Represent Business Logic for manipulation person entity
/// </summary>
public interface IPersonDeleterService
{
    /// <summary>
    /// delete person with given person id and result (true or false)
    /// </summary>
    /// <param name="personId">given person id for delete person</param>
    /// <returns>true (successful to delete) false(fail to delete)</returns>
    Task<bool> DeletePerson(Guid? personId);

}