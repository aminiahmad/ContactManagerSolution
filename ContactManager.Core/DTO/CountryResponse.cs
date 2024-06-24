using Entities;

namespace ServiceContract.DTO;

/// <summary>
/// DTO class that is used as return type for most of CountriesService Methods
/// </summary>
public class CountryResponse
{
    public Guid CountryId { get; set; }
    public string? CountryName { get; set; }

    //assert.contain method return auto equals method and contain just check type of object we implement manual this override method for check value 
    public override bool Equals(object? obj)
    {
        if(obj == null) return false;
        if (obj.GetType() != typeof(CountryResponse)) return false;
        CountryResponse other = (CountryResponse)obj;
        return CountryName==other.CountryName&&CountryId==other.CountryId ;
    }

    public override int GetHashCode()
    {
        // ReSharper disable once BaseObjectGetHashCodeCallInGetHashCode
        return base.GetHashCode();
    }
}
//extension method for country class
public static class CountryExtensions
{
    public static CountryResponse ToCountryResponse(this Country country)
    {
        return new CountryResponse()
        {
            CountryName = country.CountryName,
            CountryId = country.CountryId
        };
    }
}