using FluentAssertions;
using Microsoft.AspNetCore.Http.Features;
using Xunit;
using Fizzler;
using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;

namespace CRUDTest;

public class PersonControllerIntegrationTesting(CustomWebApplicationFactory factory)
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _httpClient = factory.CreateClient();

    [Fact]
    public async Task Index_IntegrationTesting_HttpRequest()
    {
        //arrange
        //act
        HttpResponseMessage response =await _httpClient.GetAsync("Persons/index");

        //assert
        response.Should().BeSuccessful();
        string responseBody= await response.Content.ReadAsStringAsync();

        HtmlDocument htmlDocument = new HtmlDocument();
        htmlDocument.LoadHtml(responseBody);
        var document=htmlDocument.DocumentNode;

        document.QuerySelectorAll("table.persons").Should().NotBeNull();

    }

}