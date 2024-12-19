using CityInfo.API.Modles;

namespace CityInfo.API;

public class CityDataStore
{
    public List<CityDto> Cities { get; set; }
  //  public static CityDataStore Current { get; set; } = new CityDataStore();
    public CityDataStore()
    {
        Cities = new List<CityDto>()
        {
            new CityDto()
            {
                Id=1,
                Name="Tulkarm",
                Description="This is Tulkarm",
                PointsOfIntrest =
                {
                    new PointOfIntrestDto
                    {
                        Id =1,
                        Name="Intrest1"
                    },new PointOfIntrestDto{
                        Id=2,
                        Name="Intrest2"

                    }
                }
            },
            new CityDto()
            {
                Id=2,
                Name="Nablus",
                Description="This is Nablus",
                 PointsOfIntrest =
                {
                    new PointOfIntrestDto
                    {
                        Id =3,
                        Name="Intrest1"
                    },new PointOfIntrestDto{
                        Id=4,
                        Name="Intrest2"

                    }
                }
            },
            new CityDto()
            {
                Id=3,
                Name="Jenin",
                Description="This is Jenin"
            }
        };
    }
}