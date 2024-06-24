using System.ComponentModel.DataAnnotations;
namespace Service.Helpers;

public class ValidationHelpers
{
    internal static void ModelValidation(object obj)
    {
        //validation person name must have value
        ValidationContext validationContext = new ValidationContext(obj);
        List<ValidationResult> validationResult= new List<ValidationResult>();

        bool isValid=Validator.TryValidateObject(obj, validationContext, validationResult, true);
        if (!isValid)
        {
            throw new ArgumentException(validationResult.FirstOrDefault()?.ErrorMessage);
        }
    }
}