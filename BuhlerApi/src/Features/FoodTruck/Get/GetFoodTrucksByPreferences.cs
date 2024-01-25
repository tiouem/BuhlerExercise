using System.Collections.Immutable;
using BuhlerApi.Features.Common;
using BuhlerApi.Features.Common.Data;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BuhlerApi.Features.FoodTruck.Get;

public class FoodTrucksController : ApiControllerBase
{
    [HttpGet]
    public async Task<GetFoodTrucksByPreferences.Response> GetFoodTrucksByPreferences(
        [FromQuery] double latitude,
        [FromQuery] double longitude,
        [FromQuery] int resultsToReturn,
        [FromQuery] string preferredFood
    )
    {
        return await Mediator.Send(
            new GetFoodTrucksByPreferences.Request(latitude, longitude, resultsToReturn, preferredFood));
    }
}

public static class GetFoodTrucksByPreferences
{
    public record Request(double Latitude, double Longitude, int ResultsToReturn, string PreferredFood)
        : IRequest<Response>;

    public record Response(int NumberOfResults, IEnumerable<FoodTruckDto> FoodTrucks);

    internal sealed class GetFoodTrucksByPreferencesHandler : IRequestHandler<Request, Response>
    {
        private readonly IDataProvider _dataProvider;

        public GetFoodTrucksByPreferencesHandler(IDataProvider dataProvider)
        {
            _dataProvider = dataProvider;
        }

        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            var allPermits = await _dataProvider.GetAllFoodPermits();

            var gf = NetTopologySuite.NtsGeometryServices.Instance.CreateGeometryFactory(4326);

            var currentLocation =
                gf.CreatePoint(new NetTopologySuite.Geometries.Coordinate(request.Latitude, request.Longitude));

            var nearestFoodTrucks = allPermits
                .Where(x => string.Equals(x.Status, "APPROVED", StringComparison.InvariantCultureIgnoreCase))
                .Where(x =>
                    x.FoodItems.Contains(request.PreferredFood, StringComparison.InvariantCultureIgnoreCase))
                .OrderBy(c => c.LocationAsPoint.Distance(gf.CreateGeometry(currentLocation)))
                .Take(request.ResultsToReturn)
                .Select(x => new FoodTruckDto(x.LocationId, x.Applicant, x.LocationDescription, x.Address, x.FoodItems))
                .ToImmutableList();

            return new Response(nearestFoodTrucks.Count, nearestFoodTrucks);
        }
    }
}