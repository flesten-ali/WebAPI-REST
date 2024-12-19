using Asp.Versioning;
using AutoMapper;
using CityInfo.API.Modles;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
namespace CityInfo.API.Controllers;


[ApiController]
[Route("api/v{version:apiVersion}/cities")]
 [Authorize]
[ApiVersion(1)]
[ApiVersion(2)]
public class CitiesController : ControllerBase
{
    private ICityInfoRepositiory _cityInfoRepositiory;
    private readonly IMapper _mapper;
    private const int MaxPageSize = 20;

    public CitiesController(ICityInfoRepositiory cityInfoRepositiory, IMapper mapper)
    {
        _cityInfoRepositiory = cityInfoRepositiory;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CityWithoutPointOfInterest>>> GetCities
        (string? name , string? searchQuery , int pageNumber =1 , int pageSize =10)
    {
        pageSize = (pageSize > MaxPageSize ? MaxPageSize : pageSize);

        var (cities, paginationMeteData) = await _cityInfoRepositiory
            .GetCitiesAsync(name, searchQuery, pageNumber, pageSize);

        Response.Headers.Add("X-PaginationMetaData",
            JsonSerializer.Serialize(paginationMeteData));

        return Ok(_mapper.Map<IEnumerable<CityWithoutPointOfInterest>>(cities));
    }

    /// <summary>
    /// Get City By Id
    /// </summary>
    /// <param name="cityId">The id to get the city</param>
    /// <param name="includePointOfInterest">wether or not to include the point of interest</param>
    /// <returns>the city with or without the point of interest</returns>
    ///<response code="200">Returns the requested City</response>


    [HttpGet("{cityId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
 
    public async Task<IActionResult> GetCity(int cityId, bool includePointOfInterest = false)
    {
        var city = await _cityInfoRepositiory.GetCityAsync(cityId, includePointOfInterest);

        if (city == null)
        {
            return NotFound();
        }
        if (includePointOfInterest)
        {
            return Ok(_mapper.Map<CityDto>(city));
        }
        return Ok(_mapper.Map<CityWithoutPointOfInterest>(city));
    }
}
