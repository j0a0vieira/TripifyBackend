namespace TripifyBackend.API.DTO_s;

public class GetTripRouteRequest
{
    public string StartingPoint { get; set; }
    public string? Destination { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public List<string> Categories { get; set; }
    public string TargetGroup { get; set; }
    public List<string> MandatoryToVisit { get; set; }
    public string Budget { get; set; }
}