using NetTopologySuite.Geometries;

namespace BuhlerApi.Features.FoodTruck;

public record FoodPermit(
    int LocationId,
    string Applicant,
    string LocationDescription,
    string Address,
    string Status,
    string FoodItems,
    Point LocationAsPoint);

public record FoodTruckDto(
    int LocationId,
    string Applicant,
    string LocationDescription,
    string Address,
    string FoodItems);