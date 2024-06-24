
using Entities;

namespace ServiceContract.DTO
{
    /// <summary>
    /// 
    /// </summary>
    public class CountryAddRequest
    {
        public string? CountryName { get; set; }

        public Country ToCountry()
        {
            return new Country()
            {
                CountryName = CountryName
            };
        }

    }
}
