using BuhlerApi.Features.FoodTruck;

namespace BuhlerApi.Features.Common.Data;

public interface IDataProvider
{
    Task<IEnumerable<FoodPermit>> GetAllFoodPermits();
}