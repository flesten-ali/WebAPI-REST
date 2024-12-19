namespace CityInfo.API.Modles
{
    public class CityWithoutPointOfInterest
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}