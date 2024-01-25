using System.Globalization;
using BuhlerApi.Features.FoodTruck;
using NetTopologySuite.Geometries;

namespace BuhlerApi.Features.Common.Data;

public class CsvDataProvider : IDataProvider
{
    public async Task<IEnumerable<FoodPermit>> GetAllFoodPermits()
    {
        var csvData = await File.ReadAllLinesAsync("./src/Features/Common/Data/Mobile_Food_Facility_Permit.csv");
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

        return permits;
    }
}