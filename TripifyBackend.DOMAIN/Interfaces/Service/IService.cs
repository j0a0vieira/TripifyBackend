using TripifyBackend.DOMAIN.Models;

namespace TripifyBackend.DOMAIN.Interfaces.Service;

public interface IService
{
    void FillDatabase(string lat, string lon);

    Task<List<PlaceDomain>?> GetTripRoute(string lat, string lon,
        int tripDuration,
        double maxDistance,
        List<string> categories,
        bool includeHotel,
        string travelMode,
        string targetGroup,
        List<string> mandatoryToVisit,
        string budget
    );

    List<DomainError> GetErrors();
}