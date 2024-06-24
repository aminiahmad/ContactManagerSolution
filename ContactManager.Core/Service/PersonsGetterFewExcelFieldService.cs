using OfficeOpenXml;
using ServiceContract;
using ServiceContract.DTO;

namespace Service;

public class PersonsGetterFewExcelFieldService:IPersonGetterService
{
    private readonly PersonsGetterService _personGetterService;
    public PersonsGetterFewExcelFieldService(PersonsGetterService personGetterService)
    {
        _personGetterService = personGetterService;
    }
    public  async Task<List<PersonResponse>> GetAllPersons()
    {
        return await _personGetterService.GetAllPersons();
    }

    public  async Task<PersonResponse?> GetPersonByPersonId(Guid? personId)
    {
        return await _personGetterService.GetPersonByPersonId(personId);
    }

    public  async Task<List<PersonResponse>> GetFilteredPersons(string searchBy, string? search)
    {
        return await _personGetterService.GetFilteredPersons(searchBy, search);
    }

    public async Task<MemoryStream> GetPersonsCsv()
    {
        return await _personGetterService.GetPersonsCsv();
    }

    public  async Task<MemoryStream> GetPersonsExcel()
    {
        MemoryStream memoryStream=new MemoryStream();

        using (ExcelPackage excelPackage = new ExcelPackage(memoryStream))
        {
            ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("personsSheet");

            worksheet.Cells["A1"].Value = "Person Name";
            worksheet.Cells["B1"].Value = "Gender";
            worksheet.Cells["C1"].Value = "DateOfBirth";
            using (ExcelRange headerCell = worksheet.Cells["A1:C1"])
            {
                headerCell.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                headerCell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.DarkOliveGreen);
                headerCell.Style.Font.Bold=true;
                headerCell.Style.Font.Color.SetColor(System.Drawing.Color.Beige);
            }

            List<PersonResponse> persons = await GetAllPersons();

            int rows = 2;
            foreach (var person in persons)
            {
                worksheet.Cells[rows, 1].Value = person.PersonName;
                worksheet.Cells[rows, 2].Value = person.Gender;
                if(person.DateOfBirth.HasValue)
                    worksheet.Cells[rows, 3].Value = person.DateOfBirth;
                rows++;
            }
            worksheet.Cells[$"A1:C{rows}"].AutoFitColumns();
            await excelPackage.SaveAsync();
        }
        memoryStream.Position = 0;
        return memoryStream;
    }
}