using System.Text.Json;
using TripifyBackend.DOMAIN.Interfaces.Repository;
using TripifyBackend.DOMAIN.Interfaces.Service;
using TripifyBackend.DOMAIN.Models;
using TripifyBackend.INFRA.RepositoryExceptions;

namespace TripifyBackend.APPLICATION.Services;

public class TripService : IService
{
    private new List<DomainError> _errorList = [];
    private readonly IRepository _repository;
    public TripService(IRepository repository)
    {
        _repository = repository;
    }

    public List<DomainError> GetErrors()
    {
        return _errorList;
    }

    public async void FillDatabase(string lat, string lon)
    {
        using var http = new HttpClient
        {
            BaseAddress = new Uri("https://api.foursquare.com/v3/places/")
        };
            
        http.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "fsq3ua+/Y3GwtN4ciiOooUqdCGKnFLQmM4iFcp8o9In42M0=");

        var response = await http.GetAsync($"search?ll={lat},{lon}&radius=5000");

        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            Results? place = JsonSerializer.Deserialize<Results>(content);  
                
            foreach (var placeResult in place.results)
            {
                placeResult.Id = Guid.NewGuid();
                foreach (var placeResultCategory in placeResult.categories)
                {
                    placeResultCategory.ID = Guid.NewGuid();    
                }
            }
        }
    }

    public async Task<List<PlaceDomain>?> GetTripRoute(GetTripRouteRequestDomain request)
    {
        if (!ValidationMethods.ValidateTripDuration(request.TripDuration, _errorList)) { return null; }
        if (!ValidationMethods.ValidateMaxDistance(request.MaxDistance, _errorList)) { return null; }
        if (!ValidationMethods.ValidateCategories(request.categories, _errorList)) { return null; }
        
        return null;
    }
    
    public async Task<List<CategoriesDomain>> GetAllCategories()
    {
        try
        {
            var categories = await _repository.GetAllCategories();
            return categories;
        }
        catch (SqlExceptionCustom e)
        {
            _errorList.Add(new DomainError
            {
                StatusCode = e.DomainError.StatusCode,
                Type = e.DomainError.Type,
                Field = e.DomainError.Field,
                Detail = e.DomainError.Detail,
            });
            
            return null;
        }
        catch (Exception e)
        {
            _errorList.Add(new DomainError
            {
                StatusCode = DomainError.StatusCodeEnum.InternalServerError,
                Type = DomainError.ErrorTypeEnum.Unknown,
                Field = DomainError.FieldTypeEnum.NotApplicable,
                Detail = "Unknown Internal Server Error"
            });

            return null;
        }
    }
}