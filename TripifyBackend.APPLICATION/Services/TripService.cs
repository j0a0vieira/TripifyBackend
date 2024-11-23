using System.Text.Json;
using AutoMapper;
using TripifyBackend.DOMAIN.Interfaces.Repository;
using TripifyBackend.DOMAIN.Interfaces.Service;
using TripifyBackend.DOMAIN.Models;
using TripifyBackend.INFRA.Entities.OpenAI;
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
            "56aa371be4b08b9a8d573532",
            "52e81612bcbc57f1066b79e6",
            "4bf58dd8d48988d192941735",
            "52e81612bcbc57f1066b79ed",
            "4bf58dd8d48988d132941735",
            "56aa371be4b08b9a8d5734fc",
            "52e81612bcbc57f1066b7a3f",
            "5744ccdfe4b0c0459246b4ac",
            "52e81612bcbc57f1066b7a40",
            "4bf58dd8d48988d138941735",
            "52e81612bcbc57f1066b7a41",
            "4eb1d80a4b900d56c88a45ff",
            "5bae9231bedf3950379f89c9",
            "4bf58dd8d48988d13a941735",
            "56aa371be4b08b9a8d5734f6",
            "52e81612bcbc57f1066b7a10",
            "52e81612bcbc57f1066b7a38",
            "56aa371be4b08b9a8d573544",
            "4bf58dd8d48988d1e2941735",
            "52e81612bcbc57f1066b7a22",
            "4bf58dd8d48988d1df941735",
            "4bf58dd8d48988d1e4941735",
            "50aaa49e4b90af0d42d5de11",
            "56aa371be4b08b9a8d573511",
            "5fac018b99ce226e27fe7573",
            "52e81612bcbc57f1066b7a12",
            "52e81612bcbc57f1066b7a23",
            "56aa371be4b08b9a8d573547",
            "4bf58dd8d48988d15a941735",
            "4bf58dd8d48988d1e0941735",
            "4bf58dd8d48988d159941735",
            "5bae9231bedf3950379f89cd",
            "4deefb944765f83613cdba6e",
            "4bf58dd8d48988d160941735",
            "50aaa4314b90af0d42d5de10",
            "4bf58dd8d48988d161941735",
            "4bf58dd8d48988d15d941735",
            "5642206c498e4bfca532186c",
            "4bf58dd8d48988d12d941735",
            "4eb1d4d54b900d56c88a45fc",
            "55a5a1ebe4b013909087cb77",
            "52e81612bcbc57f1066b7a13",
            "52e81612bcbc57f1066b7a30",
            "52e81612bcbc57f1066b7a14",
            "4bf58dd8d48988d163941735",
            "52e81612bcbc57f1066b7a21",
            "63be6904847c3692a84b9be0",
            "5bae9231bedf3950379f89d0",
            "63be6904847c3692a84b9be1",
            "4eb1d4dd4b900d56c88a45fd",
            "50328a4b91d4c4b30a586d6b",
            "4bf58dd8d48988d165941735",
            "4bf58dd8d48988d166941735",
            "4bf58dd8d48988d130941735",
            "5032848691d4c4b30a586d61",
            "56aa371be4b08b9a8d573560",
            "56aa371be4b08b9a8d5734c3",
            "4fbc1be21983fc883593e321",
            "5bae9231bedf3950379f89c7"
        };

        var categories = string.Join(",", categoryList);

        var response =  await http.GetAsync($"search?ll={lat},{lon}&radius=5000&categories={categories}");

        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            places = JsonSerializer.Deserialize<Results>(content);
        }
        
        var placeDomains = _mapper.Map<List<PlaceDomain>>(places.results);
        var addedPlaces = await _repository.FillDatabase(placeDomains);

        return addedPlaces;
    }

    public async Task<List<TripDomain>> GetTripRoute(DomainUserPreferences request)
    {
        /*if (!ValidationMethods.ValidateTripDuration(request.TripDuration, _errorList)) { return null; }
        if (!ValidationMethods.ValidateMaxDistance(request.MaxDistance, _errorList)) { return null; }
        if (!ValidationMethods.ValidateCategories(request.categories, _errorList)) { return null; }*/
        var places = await _repository.GetGeneratedRoute(request);
        
        return places;
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