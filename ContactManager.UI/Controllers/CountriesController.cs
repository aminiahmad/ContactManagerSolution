using Microsoft.AspNetCore.Mvc;
using ServiceContract;

namespace CRUDExample.Controllers
{
    [Route("[controller]")]
    public class CountriesController : Controller
    {
        private readonly ICountryUploaderService _countryUploaderService;

        public CountriesController(ICountryUploaderService countryUploaderService)
        {
            _countryUploaderService = countryUploaderService;
        }

        [Route("[action]")]
        public IActionResult UploadExcel()
        {
            return View();
        }

        [Route("[action]")]
        [HttpPost]
        public async Task<IActionResult> UploadExcel(IFormFile? excelFile)
        {
            if (excelFile == null || excelFile.Length==0)
            {
                ViewBag.ErrorMessage = "please select xlsx file";
                return View();

            }
            if (!Path.GetExtension(excelFile.FileName).Equals(".xlsx"))
            {
                ViewBag.ErrorMessage = "unsupported this file , 'xlsx' excepted";
                return View();
            }

            var numberOfCountryAdd= await _countryUploaderService.UploadCountriesFromExcelFile(excelFile);
            ViewBag.Message = $"{numberOfCountryAdd} Country added";
            return View();
        }
    }
}
