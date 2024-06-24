using System.Linq.Expressions;
using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RepositoryContract;

namespace Repositories;

public class PersonsRepository:IPersonsRepository
{
    private readonly ApplicationDbContext _db;
    private readonly ILogger<PersonsRepository> _logger;

    public PersonsRepository(ApplicationDbContext db , ILogger<PersonsRepository> logger)
    {
        _db=db;
        _logger=logger;
    }
    public async Task<Person> AddPerson(Person person)
    {
        await _db.Persons.AddAsync(person);
        await _db.SaveChangesAsync();
        return person;
    }

    public async Task<List<Person>> GetAllPersons()
    {
        _logger.LogInformation("GetAllPersons of personsRepository");
        return await _db.Persons.Include("Country").ToListAsync();
    }

    public async Task<Person?> GetPersonByPersonId(Guid personId)
    {
        return await _db.Persons.FirstOrDefaultAsync(temp => temp.PersonId == personId);
    }

    public async Task<List<Person>> GetFilteredPersons(Expression<Func<Person, bool>> predicate)
    {
        _logger.LogInformation("GetFilteredPersons of personsRepository");
        return await _db.Persons.Include("Country")
            .Where(predicate)
            .ToListAsync();

    }

    public async Task<bool> DeletePersonByPersonId(Guid personId)
    {
        _db.Persons.RemoveRange(_db.Persons.Where(temp => temp.PersonId == personId));

        //DELETE
        int rowsDelete= await _db.SaveChangesAsync();
        return rowsDelete > 0;
    }

    public async Task<Person> UpdatePerson(Person person)
    {
        Person? matchingPerson = await _db.Persons.FirstOrDefaultAsync(temp => temp.PersonId == person.PersonId);

        if (matchingPerson == null)
            return person;
        //update detail person with personUpdateRequest object
        matchingPerson.PersonName = person.PersonName;
        matchingPerson.Email = person.Email;
        matchingPerson.ReceiveNewsLetters = person.ReceiveNewsLetters;
        matchingPerson.Gender = person.Gender;
        matchingPerson.Address = person.Address;
        matchingPerson.DateOfBirth = person.DateOfBirth;
        matchingPerson.CountryId = person.CountryId;

        //UPDATE
        await _db.SaveChangesAsync();
        return matchingPerson;
    }
}