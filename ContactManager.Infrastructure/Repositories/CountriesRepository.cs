﻿using Entities;
using Microsoft.EntityFrameworkCore;
using RepositoryContract;

namespace Repositories
{
    public class CountriesRepository : ICountriesRepository
    {
        private readonly ApplicationDbContext _db;

        public CountriesRepository(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<Country> AddCountry(Country country)
        {
            await _db.Countries.AddAsync(country);
            
            //INSERT
            await _db.SaveChangesAsync();
            return country;
        }

        public async Task<List<Country>> GetAllCountries()
        {
           return await _db.Countries.ToListAsync();
        }

        public async Task<Country?> GetCountryById(Guid countryId)
        {
            return await _db.Countries.FirstOrDefaultAsync(temp=>temp.CountryId==countryId);
        }

        public async Task<Country?> GetCountryByName(string countryName)
        {
            return await _db.Countries.FirstOrDefaultAsync(temp => temp.CountryName == countryName);
        }
    }
}
