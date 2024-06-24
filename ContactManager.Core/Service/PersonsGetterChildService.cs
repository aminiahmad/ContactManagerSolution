using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using RepositoryContract;
using ServiceContract.DTO;

namespace Service;

public class PersonsGetterChildService:PersonsGetterService
{
    public PersonsGetterChildService(IPersonsRepository personsRepository,ILogger<PersonsGetterService> logger)
        :base(personsRepository,logger)
    {
        
    }
    public async override Task<MemoryStream> GetPersonsExcel()
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