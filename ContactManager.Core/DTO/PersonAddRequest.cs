using System.ComponentModel.DataAnnotations;
using Entities;

namespace ServiceContract.DTO;
using Enums;
/// <summary>
/// DTO for Inserting a new Person
/// </summary>
public class PersonAddRequest
{
    [Required(ErrorMessage = "person name can't be blank")]
    public string? PersonName { get; set; }
    [Required(ErrorMessage = "email value can't be blank")]
    [EmailAddress(ErrorMessage = "email value should be email address format")]
    [DataType(DataType.EmailAddress)]
    public string? Email { get; set; }
    [Required(ErrorMessage = "please select gender of person")]
    public GenderOptions? Gender { get; set; }
    [DataType(DataType.Date)]
    [Required(ErrorMessage = "Date Of Birth can't be blank")]
    public DateTime? DateOfBirth { get; set; }
    [Required(ErrorMessage = "address can't be blank")]
    public string? Address { get; set; }
    [Required(ErrorMessage = "please select country")]
    public Guid? CountryId { get; set; }
    public bool ReceiveNewsLetters { get; set; }

    /// <summary>
    /// convert the current object of PersonAddRequest into a new Person object 
    /// </summary>
    /// <returns></returns>
    public Person ToPerson()
    {
        return new Person()
        {
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