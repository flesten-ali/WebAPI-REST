using AutoMapper;
using CityInfo.API.Entities;
using CityInfo.API.Modles;

namespace CityInfo.API.Profiles
{
    public class PointOfInterestProfile:Profile
    {
        public PointOfInterestProfile()
        {
            CreateMap<PointOfInterest, PointOfIntrestDto>();
            CreateMap<PointOfInterestDtoForCreating, PointOfInterest>();
            CreateMap<PointOfInterestDtoForUpdating, PointOfInterest>();
            CreateMap<PointOfInterest, PointOfInterestDtoForUpdating>();
        }
    }
}
