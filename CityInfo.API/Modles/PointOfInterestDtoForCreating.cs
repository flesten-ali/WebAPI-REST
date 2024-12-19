using System.ComponentModel.DataAnnotations;

namespace CityInfo.API.Modles;

public class PointOfInterestDtoForCreating
{
    [Required(ErrorMessage="Name field is required")]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? Description { get; set; }
}
