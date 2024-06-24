using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ContactManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    CountryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CountryName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.CountryId);
                });

            migrationBuilder.CreateTable(
                name: "Persons",
                columns: table => new
                {
                    PersonId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PersonName = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    Gender = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CountryId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ReceiveNewsLetters = table.Column<bool>(type: "bit", nullable: false),
                    TaxIdentificationNumber = table.Column<string>(type: "varchar(8)", nullable: true, defaultValue: "ABC4576")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Persons", x => x.PersonId);
                    table.CheckConstraint("CHK_TIN", "len([TaxIdentificationNumber])=7");
                    table.ForeignKey(
                        name: "FK_Persons_Countries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Countries",
                        principalColumn: "CountryId");
                });

            migrationBuilder.InsertData(
                table: "Countries",
                columns: new[] { "CountryId", "CountryName" },
                values: new object[,]
                {
                    { new Guid("0b9e7df1-5933-4ae5-8f7f-6cd0d56a0d89"), "Usa" },
                    { new Guid("5abd89bb-3453-4149-bd2f-67a1a1f7b468"), "Iran" },
                    { new Guid("a1cbb2ec-8ce3-427b-861c-04d436e78058"), "Japan" }
                });

            migrationBuilder.InsertData(
                table: "Persons",
                columns: new[] { "PersonId", "Address", "CountryId", "DateOfBirth", "Email", "Gender", "PersonName", "ReceiveNewsLetters" },
                values: new object[,]
                {
                    { new Guid("69e6d235-6a3f-4bba-89be-fc82e7c5816a"), "wa157wdfwa", new Guid("a1cbb2ec-8ce3-427b-861c-04d436e78058"), new DateTime(1995, 12, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "awdaw12@gmail.com", "Female", "sara", false },
                    { new Guid("7c57cdb1-1ad0-41c9-88c4-4261787bd24b"), "wadawfgsrgsfef", new Guid("0b9e7df1-5933-4ae5-8f7f-6cd0d56a0d89"), new DateTime(1990, 12, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "awdwadfgeaw@gmail.com", "Male", "ahmad", true },
                    { new Guid("a42b03a8-35cd-4cf2-841d-f8445eb91475"), "wadawfgsrgsfef", new Guid("5abd89bb-3453-4149-bd2f-67a1a1f7b468"), new DateTime(1990, 12, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "awdaw@gmail.com", "Male", "reza", true }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Persons_CountryId",
                table: "Persons",
                column: "CountryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Persons");

            migrationBuilder.DropTable(
                name: "Countries");
        }
    }
}
