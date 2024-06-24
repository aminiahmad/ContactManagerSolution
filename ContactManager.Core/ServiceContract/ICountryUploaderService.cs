using Microsoft.AspNetCore.Http;
namespace ServiceContract
{
    public interface ICountryUploaderService
    {
       
        /// <summary>
        /// list of countries of excel file add to database
        /// </summary>
        /// <param name="formFile">excel file with list of country to add</param>
        /// <returns>number of countries added</returns>
        Task<int> UploadCountriesFromExcelFile(IFormFile? formFile);

    }
}
