namespace TripifyBackend.API.DTO_s;

public class GetTripRouteResponse
{
    public List<TripDTO> Trips { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}