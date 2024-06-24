using System.ComponentModel.DataAnnotations;

namespace Entities;

/// <summary>
///  Persson Domain Model Class
/// </summary>
public class Person
{
    [Key]
    public Guid? PersonId { get; set; }
    // by default nvarchar(max)
    //nvarchar(20)
    [StringLength(40)]
    public string? PersonName { get; set; }

    [StringLength(40)]
    public string? Email { get; set; }

    [StringLength(10)]
    public string? Gender { get; set; }

    public DateTime? DateOfBirth { get; set; }

    [StringLength(200)]
    public string? Address { get; set; }

    //uniqueIdentifier
    public Guid? CountryId { get; set; }

    //bit
    public bool ReceiveNewsLetters { get; set; }

    //Text Identification number 
    public string? Tin { get; set; }

    //navigation property PARENT

    public virtual Country? Country { get; set; }
}