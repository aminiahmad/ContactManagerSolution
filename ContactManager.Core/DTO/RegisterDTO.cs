using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using ServiceContract.Enums;

namespace ContactManager.Core.DTO;

public class RegisterDTO
{
    [Required(ErrorMessage = "Person name can't be blank")]
    public string? PersonName { get; set; }

    [Required(ErrorMessage = "Email can't be blank")]
    [EmailAddress(ErrorMessage = "email address should be a proper email format")]
    //remote validation (بدون اینکه درخواست بفرستیم نشون بده که همچین کاربری قبلا ثبت نام کرده)
    [Remote("IsEmailAlreadyRegistered","Account",ErrorMessage = "Email already exists!")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Phone can't be blank")]
    [RegularExpression("^[0-9]*$",ErrorMessage = "phone number should contain number only")]
    public string? Phone { get; set; }

    [Required(ErrorMessage = "Password can't be blank")]
    [DataType(DataType.Password)]
    public string? Password { get; set; }

    [Required(ErrorMessage = "ConfirmPassword can't be blank")]
    [DataType(DataType.Password)]
    [Compare("Password",ErrorMessage = "Confirm password must same with password field")]
    public string? ConfirmPassword { get; set; }

    public UserRoles Roles { get; set; }
}