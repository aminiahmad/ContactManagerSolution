
using ServiceContract;
using Entities;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using RepositoryContract;

namespace Service
{
    public class CountryUploaderService : ICountryUploaderService
    {
        private readonly ICountriesRepository _countriesRepository;

        public CountryUploaderService(ICountriesRepository countriesRepository)
        {
            _countriesRepository = countriesRepository;
            
        }
        
        public async Task<int> UploadCountriesFromExcelFile(IFormFile? formFile)
        {
            MemoryStream memoryStream=new MemoryStream();
            await formFile?.CopyToAsync(memoryStream)!;
            int rowsInserted = 0;
            using (ExcelPackage excelPackage = new ExcelPackage(memoryStream))
            {
                ExcelWorksheet worksheet= excelPackage.Workbook.Worksheets["Countries"];

                int rows = worksheet.Dimension.Rows;

                for (int row = 2; row <= rows; row++)
                {
                    string? cell= Convert.ToString(worksheet.Cells[row, 1].Value);
                    if (!string.IsNullOrEmpty(cell))
                    {
                        string? countryName = cell;

                        if (await _countriesRepository.GetCountryByName(countryName) == null)
                        {
                            Country country = new Country(){CountryName = cell};
                            await _countriesRepository.AddCountry(country);
                            rowsInserted++;
                        }
                    }
                }
            }
            return rowsInserted;
        }
    }
}
