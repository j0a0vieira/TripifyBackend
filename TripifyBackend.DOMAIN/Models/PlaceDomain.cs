namespace TripifyBackend.DOMAIN.Models;

public class PlaceDomain
{
    public Guid? Id { get; set; }
    public string Name { get; set; }
    public string Timezone { get; set; }
    public float Latitude { get; set; }
    public float Longitude { get; set; }
    public string FormattedAddress { get; set; }
    public string Country { get; set; }
    public string Locality { get; set; }
    public string Postcode { get; set; }
    public string Region { get; set; }
    public List<CategoriesDomain> Categories { get; set; }
}