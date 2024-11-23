namespace TripifyBackend.DOMAIN.Models;

public class DomainUserPreferences
{
    public float StartingLat { get; set; }
    public float StartingLon { get; set; }
    public float DestinationLat { get; set; }
    public float DestinationLon { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public List<string> Categories { get; set; }
    public string TargetGroup { get; set; }
    public List<string> MandatoryToVisit { get; set; }
    public string Budget { get; set; }
}