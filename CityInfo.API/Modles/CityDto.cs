using System.Linq;

namespace CityInfo.API.Modles;

public class CityDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public ICollection<PointOfIntrestDto> PointsOfIntrest { get; set; }
          = new List<PointOfIntrestDto>();
    public int PointOfIntrestNumber
    {
        get
        {
            return PointsOfIntrest.Count;

        }
    }
}
