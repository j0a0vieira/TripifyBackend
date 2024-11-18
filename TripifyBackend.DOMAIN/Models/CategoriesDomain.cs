namespace TripifyBackend.DOMAIN.Models;

public class CategoriesDomain
{
    public Guid? ID { get; set; }
    public string Name { get; set; }
    public List<PlaceDomain> Places { get; set; }
}