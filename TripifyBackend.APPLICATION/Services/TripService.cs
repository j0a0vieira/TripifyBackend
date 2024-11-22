using System.Text.Json;
using AutoMapper;
using TripifyBackend.DOMAIN.Interfaces.Repository;
using TripifyBackend.DOMAIN.Interfaces.Service;
using TripifyBackend.DOMAIN.Models;
using TripifyBackend.INFRA.RepositoryExceptions;

namespace TripifyBackend.APPLICATION.Services;

public class TripService : IService
{
    private new List<DomainError> _errorList = [];
    private readonly IRepository _repository;
    private readonly IMapper _mapper;

    public TripService(IRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }


    public List<DomainError> GetErrors()
    {
        return _errorList;
    }

    public async Task<List<PlaceDomain>> FillDatabase(string lat, string lon)
    {
        Results? places = new Results();
        var http = new HttpClient
        {
            BaseAddress = new Uri("https://api.foursquare.com/v3/places/")
        };

        http.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "fsq3ua+/Y3GwtN4ciiOooUqdCGKnFLQmM4iFcp8o9In42M0=");

        var categoryList = new string[] {
            "4d4b7104d754a06370d81259",
            "4d4b7105d754a06377d81259",
            "4bf58dd8d48988d132941735",
            "52e81612bcbc57f1066b7a40",
            "4bf58dd8d48988d1e2941735",
            "52e81612bcbc57f1066b7a0d",
            "52e81612bcbc57f1066b7a30"
        };

        var categories = string.Join(",", categoryList);

        var response =  await http.GetAsync($"search?ll={lat},{lon}&radius=5000&categories={categories}");

        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            places = JsonSerializer.Deserialize<Results>(content);
        }
        
        // Map and persist the results
        var placeDomains = _mapper.Map<List<PlaceDomain>>(places.results);
        var addedPlaces = await _repository.FillDatabase(placeDomains);

        return addedPlaces;
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