using System.Globalization;
using System.Web;
using BuhlerApi.Features.Common.Data;
using BuhlerApi.Features.FoodTruck;
using BuhlerApi.Features.FoodTruck.Get;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using NetTopologySuite.Geometries;
using Newtonsoft.Json.Linq;

namespace BuhlerApi.Tests.FoodTruck.Get;

[TestFixture]
public class LeagueTests
{
    // INTEGRATION
    [Test]
    public async Task GetFoodTrucksByPreferences_ReturnsCorrectData_Integration()
    {
        var csvData = await File.ReadAllLinesAsync("./FoodTruck/Get/Json/Mobile_Food_Facility_Permit.csv");
        var permits = new List<FoodPermit>();

        foreach (var line in csvData.Skip(1))
        {
            double x, y = new();
            var columns = line.Split(',');
            if (!double.TryParse(columns[14], CultureInfo.InvariantCulture, out x) &&
                !double.TryParse(columns[15], CultureInfo.InvariantCulture, out y))
            {
                break;
            }

            var permit = new FoodPermit(
                int.Parse(columns[0]),
                columns[1],
                columns[4],
                columns[5],
                columns[10],
                columns[11],
                new Point(x, y));
            permits.Add(permit);
        }

        await using var appFactory = new CustomWebApplicationFactory(services =>
        {
            services.Replace(ServiceDescriptor.Scoped(_ =>
            {
                var dataServiceMock = new Mock<IDataProvider>();
                dataServiceMock.Setup(x => x.GetAllFoodPermits())
                    .ReturnsAsync(permits);

                return dataServiceMock.Object;
            }));
        });
        var client = appFactory.CreateClient();

        var builder = new UriBuilder("http://localhost:5204/api/FoodTrucks");

        var parameters = HttpUtility.ParseQueryString(string.Empty);
        parameters["latitude"] = "6007018.02";
        parameters["longitude"] = "2104913.057";
        parameters["resultsToReturn"] = "20";
        parameters["preferredFood"] = "Meat";
        builder.Query = parameters.ToString();


        var response = await client.GetAsync(builder.Uri);
        var responseParsed = JToken.Parse(await response.Content.ReadAsStringAsync());
        var expected =
            JToken.Parse(
                await File.ReadAllTextAsync("./FoodTruck/Get/Json/GetByPreferencesResponse.json"));

        responseParsed.Should().BeEquivalentTo(expected);
    }

    // UNIT
    [Test]
    public async Task GetFoodTrucksByPreferences_CallsServiceCorrectly()
    {
        var request = new GetFoodTrucksByPreferences.Request(It.IsAny<double>(), It.IsAny<double>(), It.IsAny<int>(),
            It.IsAny<string>());
        var dataServiceMock = new Mock<IDataProvider>();
        dataServiceMock.Setup(x => x.GetAllFoodPermits()).ReturnsAsync(new List<FoodPermit>());

        var handler = new GetFoodTrucksByPreferences.GetFoodTrucksByPreferencesHandler(dataServiceMock.Object);

        var res = await handler.Handle(request, It.IsAny<CancellationToken>());

        dataServiceMock.Verify(x => x.GetAllFoodPermits(), Times.Once);
    }
}