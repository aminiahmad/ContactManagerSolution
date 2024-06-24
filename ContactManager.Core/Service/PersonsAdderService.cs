using Entities;
using Service.Helpers;
using ServiceContract;
using ServiceContract.DTO;
using Microsoft.Extensions.Logging;
using RepositoryContract;

namespace Service;

public class PersonsAdderService:IPersonAdderService
{
    private readonly IPersonsRepository _personsRepository;
    private readonly ILogger<PersonsAdderService> _logger;

    public PersonsAdderService(IPersonsRepository personsRepository,ILogger<PersonsAdderService> logger)
    {
        _personsRepository = personsRepository;
        _logger = logger;
    }

    //services
    public async Task<PersonResponse> AddPerson(PersonAddRequest? personAddRequest)
    {
        //check if personAddRequest is not null
        if (personAddRequest == null) throw new ArgumentNullException(nameof(personAddRequest));

        //validation person name must have value
        
        ValidationHelpers.ModelValidation(personAddRequest);

        //convert personAddRequest to person object
        Person person= personAddRequest.ToPerson();
        person.PersonId=Guid.NewGuid();

        //INSERT
        await _personsRepository.AddPerson(person);
        //_personsRepository.sp_InsertPerson(person);
        // convert person to personResponse
        return person.ToPersonResponse();
    }

}