using TripifyBackend.DOMAIN.Models;

namespace TripifyBackend.DOMAIN.Interfaces.Service;

public interface IService
{
    Task<List<PlaceDomain>> FillDatabase(string lat, string lon);

    Task<List<TripDomain>> GetTripRoute(DomainUserPreferences request);
    Task<List<CategoriesDomain>> GetAllCategories();

    List<DomainError> GetErrors();
}