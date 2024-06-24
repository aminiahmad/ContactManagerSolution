using Entities;
using ServiceContract.Enums;
using System.ComponentModel.DataAnnotations;

namespace ServiceContract.DTO;

public class PersonUpdateRequest
{
    public Guid? PersonId { get; set; }
    [Required(ErrorMessage = "person name can't be blank")]
    public string? PersonName { get; set; }
    [Required(ErrorMessage = "email value can't be blank")]
    [EmailAddress(ErrorMessage = "email value should be email address format")]
    public string? Email { get; set; }
    public GenderOptions? Gender { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Address { get; set; }
    public Guid? CountryId { get; set; }
    public bool ReceiveNewsLetters { get; set; }

    /// <summary>
    /// convert the current object of PersonAddRequest into a new Person object 
    /// </summary>
    /// <returns>object person</returns>
    public Person ToPerson()
    {
        return new Person()
        {
            PersonId = PersonId,
            PersonName = PersonName,
            Email = Email,
            Gender = Gender.ToString(),
            DateOfBirth = DateOfBirth,
            Address = Address,
            CountryId = CountryId,
            ReceiveNewsLetters = ReceiveNewsLetters
        };
    }
}