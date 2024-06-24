using ContactManager.Core.Domain.IdentityEntities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Entities
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser,ApplicationRole,Guid>
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        public virtual DbSet<Country> Countries { get; set; }
        public virtual DbSet<Person> Persons { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Country>().ToTable("Countries");
            modelBuilder.Entity<Person>().ToTable("Persons");

            //Seed to Countries
            string countriesJson = System.IO.File.ReadAllText("countries.json");
            List<Country>? countries = System.Text.Json.JsonSerializer.Deserialize<List<Country>>(countriesJson);

            if (countries != null)
                foreach (Country country in countries)
                    modelBuilder.Entity<Country>().HasData(country);


            //Seed to Persons
            string personsJson = System.IO.File.ReadAllText("persons.json");
            List<Person>? persons = System.Text.Json.JsonSerializer.Deserialize<List<Person>>(personsJson);

            if (persons != null)
                foreach (Person person in persons)
                    modelBuilder.Entity<Person>().HasData(person);

            //Fluent Api
            modelBuilder.Entity<Person>().Property(temp=>temp.Tin).HasColumnName("TaxIdentificationNumber")
                .HasColumnType("varchar(8)").HasDefaultValue("ABC4576");

            //unique because we use default value for this property we don't use is unique for this property

            // modelBuilder.Entity<Person>().HasIndex(temp => temp.Tin).IsUnique();

            //constraint
            modelBuilder.Entity<Person>().HasCheckConstraint("CHK_TIN", "len([TaxIdentificationNumber])=7");
        }

        public List<Person> sp_GetAllPersons()
        {
           return Persons.FromSqlRaw("EXECUTE [dbo].[GetAllPersons]").ToList();
        }

        public int sp_InsertPerson(Person person)
        {
            object[] parameters =
            [
                new SqlParameter("@PersonId", person.PersonId),
                new SqlParameter("@PersonName", person.PersonName),
                new SqlParameter("@Email", person.Email),
                new SqlParameter("@Gender", person.Gender),
                new SqlParameter("@DateOfBirth", person.DateOfBirth),
                new SqlParameter("@Address", person.Address),
                new SqlParameter("@CountryId", person.CountryId),
                new SqlParameter("@ReceiveNewsLetters", person.ReceiveNewsLetters)
            ];

            return Database.ExecuteSqlRaw("EXECUTE [dbo].[InsertPerson] @PersonId, @PersonName, @Email, @Gender, @DateOfBirth, @Address, @CountryId, @ReceiveNewsLetters", parameters);
        }
    }
}