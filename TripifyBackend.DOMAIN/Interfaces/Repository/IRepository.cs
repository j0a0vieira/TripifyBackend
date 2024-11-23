using TripifyBackend.DOMAIN.Models;

namespace TripifyBackend.DOMAIN.Interfaces.Repository;

public interface IRepository
{
    Task<List<PlaceDomain>> FillDatabase(List<PlaceDomain> places);
    Task<List<TripDomain>> GetGeneratedRoute(DomainUserPreferences userPreferences);
    Task<List<CategoriesDomain>> GetAllCategories();
}