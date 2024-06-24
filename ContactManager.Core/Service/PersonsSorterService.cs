using ServiceContract;
using ServiceContract.DTO;
using ServiceContract.Enums;
using Microsoft.Extensions.Logging;
using RepositoryContract;

namespace Service;

public class PersonsSorterService:IPersonSorterService
{
    private readonly IPersonsRepository _personsRepository;
    private readonly ILogger<PersonsSorterService> _logger;

    public PersonsSorterService(IPersonsRepository personsRepository,ILogger<PersonsSorterService> logger)
    {
        _personsRepository = personsRepository;
        _logger = logger;
    }

    //services
    public async Task<List<PersonResponse>> GetSortedPersons(List<PersonResponse> allPersons, string sortBy, SortOptions sortOption)
    {
        if (string.IsNullOrEmpty(sortBy)) return allPersons;


        //with reflection clean hast
        List<PersonResponse> sortedList = (sortBy, sortOption) switch
        {
            (nameof(PersonResponse.PersonName),SortOptions.Asc)
                =>allPersons.OrderBy(temp=>temp.PersonName,StringComparer.OrdinalIgnoreCase).ToList(),
            (nameof(PersonResponse.PersonName),SortOptions.Desc)
                =>allPersons.OrderByDescending(temp=>temp.PersonName,StringComparer.OrdinalIgnoreCase).ToList(),

            (nameof(PersonResponse.Address),SortOptions.Asc)
                =>allPersons.OrderBy(temp=>temp.Address,StringComparer.OrdinalIgnoreCase).ToList(),
            (nameof(PersonResponse.Address),SortOptions.Desc)
                =>allPersons.OrderByDescending(temp=>temp.Address,StringComparer.OrdinalIgnoreCase).ToList(),

            (nameof(PersonResponse.Email),SortOptions.Asc)
                =>allPersons.OrderBy(temp=>temp.Email,StringComparer.OrdinalIgnoreCase).ToList(),
            (nameof(PersonResponse.Email),SortOptions.Desc)
                =>allPersons.OrderByDescending(temp=>temp.Email,StringComparer.OrdinalIgnoreCase).ToList(),
            
            (nameof(PersonResponse.Gender),SortOptions.Asc)
                =>allPersons.OrderBy(temp=>temp.Gender,StringComparer.OrdinalIgnoreCase).ToList(),
            (nameof(PersonResponse.Gender),SortOptions.Desc)
                =>allPersons.OrderByDescending(temp=>temp.Gender,StringComparer.OrdinalIgnoreCase).ToList(),
            (nameof(PersonResponse.Country),SortOptions.Asc)
                =>allPersons.OrderBy(temp=>temp.Gender,StringComparer.OrdinalIgnoreCase).ToList(),
            (nameof(PersonResponse.Country),SortOptions.Desc)
                =>allPersons.OrderByDescending(temp=>temp.Gender,StringComparer.OrdinalIgnoreCase).ToList(),


            (nameof(PersonResponse.Age),SortOptions.Asc)
                =>allPersons.OrderBy(temp=>temp.Age).ToList(),
            (nameof(PersonResponse.Age),SortOptions.Desc)
                =>allPersons.OrderByDescending(temp=>temp.Age).ToList(),

            (nameof(PersonResponse.DateOfBirth),SortOptions.Asc)
                =>allPersons.OrderBy(temp=>temp.DateOfBirth).ToList(),
            (nameof(PersonResponse.DateOfBirth),SortOptions.Desc)
                =>allPersons.OrderByDescending(temp=>temp.DateOfBirth).ToList(),

            (nameof(PersonResponse.ReceiveNewsLetters),SortOptions.Asc)
                =>allPersons.OrderBy(temp=>temp.ReceiveNewsLetters).ToList(),
            (nameof(PersonResponse.ReceiveNewsLetters),SortOptions.Desc)
                =>allPersons.OrderByDescending(temp=>temp.ReceiveNewsLetters).ToList(),


            _=>allPersons
        };
        return sortedList;
    }

}