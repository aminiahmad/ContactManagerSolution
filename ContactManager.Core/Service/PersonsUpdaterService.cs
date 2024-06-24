using Entities;
using Service.Helpers;
using ServiceContract;
using ServiceContract.DTO;
using Exceptions;
using Microsoft.Extensions.Logging;
using RepositoryContract;

namespace Service;

public class PersonsUpdaterService:IPersonUpdaterService
{
    private readonly IPersonsRepository _personsRepository;
    private readonly ILogger<PersonsUpdaterService> _logger;

    public PersonsUpdaterService(IPersonsRepository personsRepository,ILogger<PersonsUpdaterService> logger)
    {
        _personsRepository = personsRepository;
        _logger = logger;
    }


    //services

    public async Task<PersonResponse> GetUpdatePerson(PersonUpdateRequest? personUpdateRequest)
    {
        if(personUpdateRequest==null)
            throw new ArgumentNullException(nameof(personUpdateRequest));

        //validation
        ValidationHelpers.ModelValidation(personUpdateRequest);

        //given matching person object with personId
        Person? matchingPerson=await _personsRepository.GetPersonByPersonId(personUpdateRequest.PersonId!.Value);
        if (matchingPerson == null)
            throw new InvalidPersonIdException("given person id doesn't exist ");  // Custom Exception

        //update detail person with personUpdateRequest object
        matchingPerson.PersonName = personUpdateRequest.PersonName;
        matchingPerson.Email = personUpdateRequest.Email;
        matchingPerson.ReceiveNewsLetters = personUpdateRequest.ReceiveNewsLetters;
        matchingPerson.Gender = personUpdateRequest.Gender.ToString();
        matchingPerson.Address = personUpdateRequest.Address;
        matchingPerson.DateOfBirth = personUpdateRequest.DateOfBirth;
        matchingPerson.CountryId = personUpdateRequest.CountryId;
        await _personsRepository.UpdatePerson(matchingPerson);
        return matchingPerson.ToPersonResponse();

    }

}