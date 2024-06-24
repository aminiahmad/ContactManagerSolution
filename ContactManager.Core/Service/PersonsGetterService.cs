using System.Globalization;
using CsvHelper;
using Entities;
using ServiceContract;
using ServiceContract.DTO;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using RepositoryContract;

namespace Service;

public class PersonsGetterService:IPersonGetterService
{
    private readonly IPersonsRepository _personsRepository;
    private readonly ILogger<PersonsGetterService> _logger;

    public PersonsGetterService(IPersonsRepository personsRepository,ILogger<PersonsGetterService> logger)
    {
        _personsRepository = personsRepository;
        _logger = logger;
    }

    //services

    public virtual async Task<List<PersonResponse>> GetAllPersons()
    {
        _logger.LogInformation("GetAllPersons of personsService");
        //select *
        var persons = await _personsRepository.GetAllPersons();
        return persons.Select(temp => temp.ToPersonResponse()).ToList();

        //return _personsRepository.sp_GetAllPersons().Select(temp => ConvertPersonToPersonResponse(temp)).ToList();
    }

    public virtual async Task<PersonResponse?> GetPersonByPersonId(Guid? personId)
    {
        if (personId == null) return null;

        Person? person= await _personsRepository.GetPersonByPersonId(personId.Value);
        if(person==null) return null;

        return person.ToPersonResponse();
    }

    public virtual async Task<List<PersonResponse>> GetFilteredPersons(string searchBy, string? search)
    {
        _logger.LogInformation("GetFilteredPersons of personsService");

        List<Person> matchingPersons = searchBy switch
        {
            nameof(PersonResponse.PersonName) =>
                matchingPersons = await _personsRepository.GetFilteredPersons(temp =>
                    temp.PersonName.Contains(search)),

            nameof(PersonResponse.Email) =>
                matchingPersons = await _personsRepository.GetFilteredPersons(temp =>
                    temp.Email.Contains(search)),

            nameof(PersonResponse.Gender) =>
                matchingPersons = await _personsRepository.GetFilteredPersons(temp =>
                    temp.Gender.Contains(search)),

            nameof(PersonResponse.DateOfBirth) =>
                matchingPersons = await _personsRepository.GetFilteredPersons(temp =>
                    temp.DateOfBirth.Value.ToString("yy-MM-dd").Contains(search)),

            nameof(PersonResponse.CountryId) =>
                matchingPersons = await _personsRepository.GetFilteredPersons(temp =>
                    temp.Country.CountryName.Contains(search)),

            nameof(PersonResponse.Address) =>
                matchingPersons = await _personsRepository.GetFilteredPersons(temp =>
                    temp.Address.Contains(search)),

            _ => await _personsRepository.GetAllPersons()
        };

        return matchingPersons.Select(temp=>temp.ToPersonResponse()).ToList();
    }

    public virtual async Task<MemoryStream> GetPersonsCsv()
    {
        MemoryStream memoryStream = new MemoryStream();
        StreamWriter streamWriter = new StreamWriter(memoryStream);
        using (CsvWriter csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture, leaveOpen: true))
        {

            //csvWriter.WriteHeader<PersonResponse>(); //PersonID,PersonName,...
            csvWriter.WriteField(nameof(PersonResponse.PersonName));
            csvWriter.WriteField(nameof(PersonResponse.Email));
            csvWriter.WriteField(nameof(PersonResponse.Age));
            csvWriter.WriteField(nameof(PersonResponse.Gender));
            csvWriter.WriteField(nameof(PersonResponse.DateOfBirth));
            csvWriter.WriteField(nameof(PersonResponse.Address));

            await csvWriter.NextRecordAsync();
            var persons = await GetAllPersons();
            //await csvWriter.WriteRecordsAsync(persons);
            //1,abc,....
            foreach (var person in persons)
            { 
                csvWriter.WriteField(person.PersonName);
                csvWriter.WriteField(person.Email);
                csvWriter.WriteField(person.Age);
                csvWriter.WriteField(person.Gender);
                if(person.DateOfBirth.HasValue)
                    csvWriter.WriteField(person.DateOfBirth.Value.ToString("yyyy MMMM dd"));
                else
                    csvWriter.WriteField("");
                csvWriter.WriteField(person.Address);
                await csvWriter.NextRecordAsync();
                await csvWriter.FlushAsync();
            }
        }
        memoryStream.Position = 0;
        return memoryStream;
    }

    public virtual async Task<MemoryStream> GetPersonsExcel()
    {
        MemoryStream memoryStream=new MemoryStream();

        using (ExcelPackage excelPackage = new ExcelPackage(memoryStream))
        {
            ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("personsSheet");

            worksheet.Cells["A1"].Value = "Person Name";
            worksheet.Cells["B1"].Value = "Email";
            worksheet.Cells["C1"].Value = "Address";
            worksheet.Cells["D1"].Value = "Gender";
            worksheet.Cells["E1"].Value = "DateOfBirth";
            worksheet.Cells["F1"].Value = "Country";
            using (ExcelRange headerCell = worksheet.Cells["A1:F1"])
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
                worksheet.Cells[rows, 2].Value = person.Email;
                worksheet.Cells[rows, 3].Value = person.Address;
                worksheet.Cells[rows, 4].Value = person.Gender;
                if(person.DateOfBirth.HasValue)
                    worksheet.Cells[rows, 5].Value = person.DateOfBirth;
                worksheet.Cells[rows, 6].Value = person.Country;
                rows++;
            }
            worksheet.Cells[$"A1:F{rows}"].AutoFitColumns();
            await excelPackage.SaveAsync();
        }
        memoryStream.Position = 0;
        return memoryStream;
    }
}