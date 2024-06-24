using Entities;
using ServiceContract;
using Microsoft.Extensions.Logging;
using RepositoryContract;

namespace Service;

public class PersonsDeleterService:IPersonDeleterService
{
    private readonly IPersonsRepository _personsRepository;
    private readonly ILogger<PersonsDeleterService> _logger;

    public PersonsDeleterService(IPersonsRepository personsRepository,ILogger<PersonsDeleterService> logger)
    {
        _personsRepository = personsRepository;
        _logger = logger;
    }

    //services
    public async Task<bool> DeletePerson(Guid? personId)
    {
        if (personId == null)
        {
            throw new ArgumentNullException(nameof(personId));
        }

        Person? person = await _personsRepository.GetPersonByPersonId(personId.Value);

        if (person == null)
            return false;

        return await _personsRepository.DeletePersonByPersonId(personId.Value);
    }

}