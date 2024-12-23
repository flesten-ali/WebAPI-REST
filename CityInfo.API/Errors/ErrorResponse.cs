namespace CityInfo.API.Errors;

public class ErrorResponse
{
    public List<ErrorModel> Errors { get; set; } = new List<ErrorModel>();
}
