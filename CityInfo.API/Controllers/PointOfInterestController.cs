using Asp.Versioning;
using AutoMapper;
using CityInfo.API.Entities;
using CityInfo.API.Modles;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers;

[Route("api/v{version:apiVersion}/cities/{cityId}/pointsofinterest")]
[ApiController]
//[Authorize(Policy = "MustBeFromCity")]
[ApiVersion(2)]

public class PointOfInterestController : ControllerBase
{

    private ILogger<PointOfInterestController> _logger;
    private IMailService _mailService;
    private ICityInfoRepositiory _repositiory;
    private IMapper _mapper;

    public PointOfInterestController(
        ILogger<PointOfInterestController> logger,
        IMailService mailService,
        ICityInfoRepositiory repositiory,
        IMapper mapper
        )
    {
        _logger = logger;
        _mailService = mailService;
        _repositiory = repositiory;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PointOfIntrestDto>>> GetPointsOfInterest(int cityId)
    {
        var cityName = User.Claims.FirstOrDefault(x => x.Type == "city")?.Value;
        if (!await _repositiory.CityNameMatchesCityId(cityName, cityId))
        {
            return Forbid();
        }

        if (!await _repositiory.CityExistsAsync(cityId))
        {
            _logger.LogInformation($"City with {cityId} is not Exist!");
            return NotFound();
        }

        var pointsOfInterest = await _repositiory.GetPointsOfInterestForCityAsync(cityId);
        return Ok(_mapper.Map<IEnumerable<PointOfIntrestDto>>(pointsOfInterest));
    }

    [HttpGet("{pointOfInterestId}", Name = "GetPointOfInterest")]
    public async Task<ActionResult<IEnumerable<PointOfIntrestDto>>> GetPointOfInterest
        (int cityId, int pointOfInterestId)
    {
        if (!await _repositiory.CityExistsAsync(cityId))
        {
            _logger.LogInformation($"City with {cityId} is not Exist!");
            return NotFound();
        }

        var pointOfInterest = await _repositiory.GetPointOfInterestForCityAsync(cityId, pointOfInterestId);
        if (pointOfInterest == null)
        {
            return NotFound();
        }
        return Ok(_mapper.Map<PointOfIntrestDto>(pointOfInterest));
    }

    [HttpPost]
    public async Task<ActionResult<PointOfIntrestDto>> CreatePointOfInterest
        (int cityId,
       [FromBody] PointOfInterestDtoForCreating dto)
    {
        //if (!ModelState.IsValid)
        //{
        //    return BadRequest(ModelState);
        //}

        if (!await _repositiory.CityExistsAsync(cityId))
        {
            return NotFound();
        }
        var finalPointToAdd = _mapper.Map<PointOfInterest>(dto);

        await _repositiory.AddPointOfInterestForACityAsync(cityId, finalPointToAdd);
        await _repositiory.SaveChangesAsync();

        var finalPointToReturn = _mapper.Map<PointOfIntrestDto>(finalPointToAdd);

        return CreatedAtRoute("GetPointOfInterest",
            new { cityId = cityId, pointOfInterestId = finalPointToReturn.Id }, finalPointToReturn);
    }

    [HttpPut("{pointOfInterestId}")]
    public async Task<ActionResult> UpdatePointOfInterest(int cityId,
        int pointOfInterestId,
        PointOfInterestDtoForUpdating dto)
    {
        if (!await _repositiory.CityExistsAsync(cityId))
        {
            return NotFound();
        }

        var pointEntityToUpdate = await _repositiory.GetPointOfInterestForCityAsync(cityId, pointOfInterestId);
        if (pointEntityToUpdate == null)
        {
            return NotFound();
        }
        _mapper.Map(dto, pointEntityToUpdate);
        await _repositiory.SaveChangesAsync();
        return NoContent();
    }

    [HttpPatch("{pointOfInterestId}")]
    public async Task<ActionResult> PatchPointOfInterest
    (
        int cityId,
        int pointOfInterestId,
        JsonPatchDocument<PointOfInterestDtoForUpdating> jsonPatch)
    {
        if (!await _repositiory.CityExistsAsync(cityId))
        {
            return NotFound();
        }

        var pointEntityNeedsUpdate = await _repositiory.GetPointOfInterestForCityAsync(cityId, pointOfInterestId);
        if (pointEntityNeedsUpdate == null)
        {
            return NotFound();
        }

        var pointOfInterestDtoForPatch = _mapper.Map<PointOfInterestDtoForUpdating>(pointEntityNeedsUpdate);

        jsonPatch.ApplyTo(pointOfInterestDtoForPatch, ModelState);

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        if (!TryValidateModel(pointOfInterestDtoForPatch))
        {
            return BadRequest(ModelState);
        }
        _mapper.Map(pointOfInterestDtoForPatch, pointEntityNeedsUpdate);
        await _repositiory.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{pointOfInterestId}")]
    public async Task<ActionResult> Delete(int cityId, int pointOfInterestId)
    {
        if (!await _repositiory.CityExistsAsync(cityId))
        {
            return NotFound();
        }

        var pointEntityToDelete = await _repositiory.GetPointOfInterestForCityAsync(cityId, pointOfInterestId);
        if (pointEntityToDelete == null)
        {
            return NotFound();
        }
        _repositiory.DeletePointOfInterest(pointEntityToDelete);
        await _repositiory.SaveChangesAsync();

        return NoContent();
    }
}

