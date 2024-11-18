namespace TripifyBackend.INFRA.Entities;

public class CategoriesDB
{
    public Guid? ID { get; set; }
    public string Name { get; set; }
    public List<PlaceDB> Places { get; set; }
}