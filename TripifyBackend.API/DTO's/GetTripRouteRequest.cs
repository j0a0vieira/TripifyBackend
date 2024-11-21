namespace TripifyBackend.API.DTO_s;

public class GetTripRouteRequest
{
    public string StartingPoint { get; set; }
    public string? EndingPoint { get; set; }
    public DateTime startDate { get; set; }
    public DateTime endDate { get; set; }
    public int TripDuration { get; set; }
    public int MaxDistance { get; set; }
    public List<string> categories { get; set; }
    public bool IncludeHotel { get; set; }
    public string TravelMode { get; set; }
    public string TargetGroup { get; set; }
    public List<string> mandatoryToVisit { get; set; }
    public string budget { get; set; }
}