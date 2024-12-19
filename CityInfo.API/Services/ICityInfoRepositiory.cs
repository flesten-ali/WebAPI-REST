using CityInfo.API.Entities;

namespace CityInfo.API.Services;

public interface ICityInfoRepositiory
{
    Task<IEnumerable<City>> GetCitiesAsync();
    Task<(IEnumerable<City>,PaginationMetaData)> GetCitiesAsync(string? name,string? searchQuery, int pageNumber , int pageSize);
    Task<City?> GetCityAsync(int cityId , bool includePointOfInterest);
    Task<IEnumerable<PointOfInterest>> GetPointsOfInterestForCityAsync(int cityId);
    Task<PointOfInterest?> GetPointOfInterestForCityAsync(int cityId,int pointOfInterestId);
    Task<bool> CityExistsAsync(int cityId);
    Task AddPointOfInterestForACityAsync(int cityId , PointOfInterest pointOfInterest);
    Task<bool> SaveChangesAsync();
    void DeletePointOfInterest(PointOfInterest point);
    Task<bool> CityNameMatchesCityId(string? cityName , int cityId);
}
