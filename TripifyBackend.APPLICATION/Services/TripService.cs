using System.Text.Json;
using TripifyBackend.DOMAIN.Interfaces.Service;
using TripifyBackend.DOMAIN.Models;

namespace TripifyBackend.APPLICATION.Services;

public class TripService : IService
{
    private new List<DomainError> _errorList = [];
    
    public TripService()
    {
        
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

    public async Task<List<PlaceDomain>?> GetTripRoute(string lat, string lon, 
        int tripDuration, //given in hours
        double maxDistance, //given in kms
        List<string> categories, 
        bool includeHotel,
        string travelMode, 
        string targetGroup, 
        List<string> mandatoryToVisit, 
        string budget)
    {
        if (!ValidationMethods.IsLatAndLonValid(lat, lon, _errorList)) { return null; }
        if (!ValidationMethods.ValidateTripDuration(tripDuration, _errorList)) { return null; }
        if (!ValidationMethods.ValidateMaxDistance(maxDistance, _errorList)) { return null; }
        if (!ValidationMethods.ValidateCategories(categories, _errorList)) { return null; }
        
        


        return null;
    }
}