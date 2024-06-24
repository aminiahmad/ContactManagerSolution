using System.ComponentModel.DataAnnotations;

namespace ContactManager.Core.DTO;

public class LoginDTO
{
    [Required(ErrorMessage = "Email can't be blank")]
    [EmailAddress(ErrorMessage = "input value should be match with email address format")]
    [DataType(DataType.EmailAddress)]
    public string? Email { get; set; }
    [Required(ErrorMessage = "Password can't be blank")]
    [DataType(DataType.Password)]
    public string? Password { get; set; }
}