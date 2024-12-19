using AutoMapper;
using CityInfo.API.Entities;
using CityInfo.API.Modles;

namespace CityInfo.API.Profiles;

public class CityProfile : Profile
{
    public CityProfile()
    {
        CreateMap<City, CityWithoutPointOfInterest>();
        CreateMap<City, CityDto>();
    }
}

