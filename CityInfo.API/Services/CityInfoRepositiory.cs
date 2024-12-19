using CityInfo.API.Entities;
using Microsoft.EntityFrameworkCore;
namespace CityInfo.API.Services;

public class CityInfoRepositiory : ICityInfoRepositiory
{
    private readonly CityInfoContext _context;

    public CityInfoRepositiory(CityInfoContext context)
    {
        _context = context;
    }

    public async Task AddPointOfInterestForACityAsync(int cityId, PointOfInterest pointOfInterest)
    {
        var city = await GetCityAsync(cityId, false);
        if (city != null)
        {
            city.PointsOfIntrest.Add(pointOfInterest);
        }
    }

    public async Task<bool> CityExistsAsync(int cityId)
    {
        return await _context.Cities.AnyAsync(x => x.Id == cityId);
    }

    public async Task<bool> CityNameMatchesCityId(string? cityName, int cityId)
    {
        return await _context.Cities.Where(x=>x.Name == cityName && x.Id == cityId).AnyAsync();
    }

    public void DeletePointOfInterest(PointOfInterest point)
    {
        _context.PointOfInterests.Remove(point);
    }

    public async Task<IEnumerable<City>> GetCitiesAsync()
    {
        return await _context.Cities.OrderBy(c => c.Name).ToListAsync();
    }

    public async Task<(IEnumerable<City>,PaginationMetaData)> GetCitiesAsync
        (string? name, string? searchQuery, int pageNumber, int pageSize)
    {
        var cities = _context.Cities as IQueryable<City>;

        if (!string.IsNullOrEmpty(name))
        {
            name = name.Trim();
            cities = cities.Where(x => x.Name == name);
        }

        if (!string.IsNullOrEmpty(searchQuery))
        {
            searchQuery = searchQuery.Trim();
            cities = cities.Where(x =>
               x.Name.Contains(searchQuery)
            ||
            (x.Description != null && x.Description.Contains(searchQuery)));
        }
        var totalItemsCount = await cities.CountAsync();
        var paginationMetaData = new PaginationMetaData(totalItemsCount, pageSize,pageNumber);
     
        var res =  await cities
            .OrderBy(x => x.Name)
            .Skip(pageSize*(pageNumber-1))
            .Take(pageSize)
            .ToListAsync();

        return (res, paginationMetaData);
    }

    public async Task<City?> GetCityAsync(int cityId, bool includePointOfInterest)
    {
        if (includePointOfInterest)
        {
            return await _context.Cities.Include(c => c.PointsOfIntrest).FirstOrDefaultAsync(x => x.Id == cityId);
        }
        return await _context.Cities.FirstOrDefaultAsync(x => x.Id == cityId);
    }

    public async Task<PointOfInterest?> GetPointOfInterestForCityAsync(int cityId, int pointOfInterestId)
    {
        return await _context.PointOfInterests.FirstOrDefaultAsync(p => p.Id == pointOfInterestId && p.CityId == cityId);
    }

    public async Task<IEnumerable<PointOfInterest>> GetPointsOfInterestForCityAsync(int cityId)
    {
        return await _context.PointOfInterests.Where(c => c.CityId == cityId).ToListAsync();
    }

    public async Task<bool> SaveChangesAsync()
    {
        return (await _context.SaveChangesAsync() >= 0);
    }
}
