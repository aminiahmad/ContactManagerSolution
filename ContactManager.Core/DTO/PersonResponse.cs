using Entities;
using ServiceContract.Enums;

namespace ServiceContract.DTO;

public class PersonResponse
{
    public Guid? PersonId { get; set; }
    public string? PersonName { get; set; }
    public string? Email { get; set; }
    public string? Gender { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Address { get; set; }
    public Guid? CountryId { get; set; }
    public string? Country { get; set; }
    public bool ReceiveNewsLetters { get; set; }
    public double? Age { get; set; }

    public override bool Equals(object? obj)
    {
        if (obj == null) return false;
        if(obj.GetType()!=typeof(PersonResponse)) return false;

        PersonResponse person = (PersonResponse)obj;

        return person.PersonId==PersonId&person.PersonName==PersonName&person.Email==Email&person.Gender==Gender
               &person.Address==Address&person.DateOfBirth==DateOfBirth&person.CountryId==CountryId
               &person.ReceiveNewsLetters==ReceiveNewsLetters;
    }

    public override int GetHashCode()
    {
        // ReSharper disable once BaseObjectGetHashCodeCallInGetHashCode
        return base.GetHashCode();
    }

    public override string ToString()
    {
        return $"personId:{PersonId} , personName:{PersonName}, dateOfBirth:{DateOfBirth?.ToString("MM/dd/yyyy")}" +
               $", country:{Country}, countryId:{CountryId}, gender:{Gender}, email:{Email}";
    }

    public PersonUpdateRequest ToPersonUpdate()
    {
        return new PersonUpdateRequest()
        {
            PersonId = PersonId,PersonName = PersonName,Address = Address,CountryId = CountryId,DateOfBirth = DateOfBirth
            ,Email = Email,ReceiveNewsLetters = ReceiveNewsLetters
            ,Gender =(GenderOptions)Enum.Parse((typeof(GenderOptions)),Gender!,true)
        };
    }
}

public static class PersonExtensions
{
    public static PersonResponse ToPersonResponse(this Person person)
    {
        return new PersonResponse()
        {
            PersonName = person.PersonName,Email = person.Email,Gender = person.Gender,Address = person.Address
            ,CountryId = person.CountryId,DateOfBirth = person.DateOfBirth,PersonId = person.PersonId
            ,ReceiveNewsLetters = person.ReceiveNewsLetters
            ,Age = (person.DateOfBirth!=null)?Math.Round((DateTime.Now-person.DateOfBirth.Value).TotalDays/365):null,
            Country = person.Country?.CountryName
        };
    }
}
