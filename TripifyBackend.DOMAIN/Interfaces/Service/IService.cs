using TripifyBackend.DOMAIN.Models;

namespace TripifyBackend.DOMAIN.Interfaces.Service;

public interface IService
{
    void FillDatabase(string lat, string lon);

    Task<List<PlaceDomain>?> GetTripRoute(GetTripRouteRequestDomain request);
    Task<List<CategoriesDomain>> GetAllCategories();

    List<DomainError> GetErrors();
}