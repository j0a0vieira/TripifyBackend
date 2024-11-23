using System.Net.Http.Headers;
using System.Text;
using AutoMapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TripifyBackend.DOMAIN.Interfaces.Repository;
using TripifyBackend.DOMAIN.Models;
using TripifyBackend.INFRA.DBContext;
using TripifyBackend.INFRA.Entities;
using TripifyBackend.INFRA.Entities.OpenAI;
using TripifyBackend.INFRA.RepositoryExceptions;

namespace TripifyBackend.INFRA;

public class Repository : IRepository
{
    private readonly TripifyDBContext _context;
    private readonly IMapper _mapper;
    private readonly string _openAiKey;
    private readonly string _openAiUrl;

    public Repository(TripifyDBContext context, IMapper mapper, IConfiguration configuration )
    {
        _openAiKey = configuration["OpenAI:Secret"] ?? throw new InvalidOperationException();
        _openAiUrl = configuration["OpenAI:Url"] ?? throw new InvalidOperationException();
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    } 

    public async Task<List<PlaceDomain>> FillDatabase(List<PlaceDomain> places)
    {
        var addedPlaces = new List<PlaceDB>();

        try
        {
            foreach (var placeDomain in places)
            {
                var db = _mapper.Map<PlaceDB>(placeDomain);

                var uniqueCategories = new List<CategoriesDB>();

                foreach (var category in placeDomain.Categories)
                {
                    var existingCategory = await _context.Categories
                        .FirstOrDefaultAsync(c => c.Name == category.Name);

                    if (existingCategory != null)
                    {
                        uniqueCategories.Add(existingCategory);
                    }
                    else
                    {
                        var newCategory = new CategoriesDB { Name = category.Name };
                        await _context.Categories.AddAsync(newCategory);
                        uniqueCategories.Add(newCategory);
                    }
                }

                db.Categories = uniqueCategories;

                if (!await VerifyDuplicatePlace(db))
                {
                    await _context.Places.AddAsync(db);
                    addedPlaces.Add(db);
                }
            }

            await _context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

        return places;
    }

    public async Task<List<TripDomain>> GetGeneratedRoute(DomainUserPreferences userPreferences)
    {
        var placesToReturn = new List<TripDB>();
        var placesToSearch = await GetOpenAIRoadTripPlan(userPreferences);

        foreach (var trip in placesToSearch)
        {
            var tripToAdd = new TripDB
            {
                Places = []
            };
            
            foreach (var id in trip)
            {
                var place = await _context.Places.FindAsync(Guid.Parse(id));
                if (place != null)
                {
                    tripToAdd.Places.Add(place);
                }
                
            }
            
            placesToReturn.Add(tripToAdd);
        }

        return _mapper.Map<List<TripDomain>>(placesToReturn);
    }

    public async Task<List<CategoriesDomain>> GetAllCategories()
    {
        try
        {
            var excludedCategoryIds = new List<Guid>
        {
            Guid.Parse("343d5662-3323-4d00-a03a-08dd0b8c954a"), // American Restaurant
            Guid.Parse("2948ff8d-0019-4367-a009-08dd0b8c954a"), // Asian Restaurant
            Guid.Parse("77fc94be-e8dd-4c5a-9f5a-08dd0b8c954a"), // Bagel Shop
            Guid.Parse("347a1ec4-055f-4da9-9f29-08dd0b8c954a"), // Bakery
            Guid.Parse("ff7ffce7-d046-4bc3-9f12-08dd0b8c954a"), // Bar
            Guid.Parse("509b9d40-61c7-4bad-9f6e-08dd0b8c954a"), // BBQ Joint
            Guid.Parse("9b5661b6-b844-42ae-9f31-08dd0b8c954a"), // Cafe
            Guid.Parse("2b2e16ec-8ba3-47fc-9f26-08dd0b8c954a"), // Coffee Shop
            Guid.Parse("df7338ba-3077-46a1-9f14-08dd0b8c954a"), // Deli
            Guid.Parse("78077e71-07ae-4f73-9f0b-08dd0b8c954a"), // Dessert Shop
            Guid.Parse("2357bcee-2be6-42d4-9f15-08dd0b8c954a"), // Fast Food Restaurant
            Guid.Parse("60438c3a-f0fa-4b08-9f32-08dd0b8c954a"), // Food Truck
            Guid.Parse("e93f4633-9b99-47a3-9f18-08dd0b8c954a"), // French Restaurant
            Guid.Parse("13f68e5f-6e40-43f4-9f0e-08dd0b8c954a"), // Greek Restaurant
            Guid.Parse("ae91b3a4-4ab8-45f4-9f25-08dd0b8c954a"), // Indian Restaurant
            Guid.Parse("518b7ef9-c0a2-43f6-9f19-08dd0b8c954a"), // Italian Restaurant
            Guid.Parse("6d6508b5-4f42-4e72-9f2a-08dd0b8c954a"), // Japanese Restaurant
            Guid.Parse("7b9d4e5b-b8d7-4951-9f22-08dd0b8c954a"), // Mexican Restaurant
            Guid.Parse("4d8b5b17-776e-48f4-9f27-08dd0b8c954a"), // Pizza Place
            Guid.Parse("3a15c0dc-761f-4c23-9f23-08dd0b8c954a"), // Seafood Restaurant
            Guid.Parse("4e286733-8cbf-4e3d-9f10-08dd0b8c954a"), // Steakhouse
            Guid.Parse("02ac487f-d0de-47ec-9f2b-08dd0b8c954a"), // Sushi Restaurant
            Guid.Parse("7cd8c6f8-f340-4c94-9f17-08dd0b8c954a"), // Thai Restaurant
            Guid.Parse("88ef4605-0f7f-46b2-9f28-08dd0b8c954a"), // Vegan Restaurant
            Guid.Parse("dcd1a5b0-2e1b-4590-9f24-08dd0b8c954a"), // Vegetarian Restaurant
            Guid.Parse("a3bbf6d2-94f1-41a0-9f16-08dd0b8c954a"),  // Vietnamese Restaurant
            Guid.Parse("bfcaaf24-48f9-41a3-9f40-08dd0b8c954a"), //pt restaurant
            Guid.Parse("838ff12e-989a-4a49-9f60-08dd0b8c954a"), //night club
            Guid.Parse("d79ec876-1e88-413e-9faf-08dd0b8c954a"), //pub
            Guid.Parse("91834716-28bf-4d86-9fa9-08dd0b8c954a"),
            Guid.Parse("b2901a3a-4fd0-49d6-9f87-08dd0b8c954a"),
            Guid.Parse("508746f3-0db8-41fb-9f14-08dd0b8c954a"),
            Guid.Parse("595af998-f29f-4fa2-9f42-08dd0b8c954a"),
            Guid.Parse("394dfdd7-3afd-49d7-9fe1-08dd0b8c954a"), //chinese restaurant
            Guid.Parse("e0c9cc26-c074-43a1-9f27-08dd0b8c954a"), //restaurant
            Guid.Parse("400d03ad-cc7b-46bf-9fc1-08dd0b8c954a"), //bistro
            Guid.Parse("e74321c2-d3f7-4514-9f6f-08dd0b8c954a"), //pizzaria
            Guid.Parse("a505a34a-ac21-40bb-9f43-08dd0b8c954a"), //tapas
            Guid.Parse("e5153fb2-7226-4d92-9f83-08dd0b8c954a"), //brewery
            Guid.Parse("c3ed42aa-7577-4327-9f6b-08dd0b8c954a"), //italian restaurant
            Guid.Parse("5c5b0a9a-7209-4009-9fa7-08dd0b8c954a"), // gastro pub
            Guid.Parse("d5c30740-351a-4866-9faa-08dd0b8c954a"), //auto service ?????
            Guid.Parse("13aef974-3b67-4c5d-9f63-08dd0b8c954a"), //agro service
            Guid.Parse("65320c35-ad3d-4dda-a042-08dd0b8c954a"),
            Guid.Parse("72479ea2-1615-4755-9f5c-08dd0b8c954a"),
            Guid.Parse("8a424109-c141-4080-9fd7-08dd0b8c954a"),
            Guid.Parse("47f9a88a-d7df-4cd9-a02a-08dd0b8c954a"),
            Guid.Parse("4e652bf2-e4d5-40f7-9fb5-08dd0b8c954a"),
            Guid.Parse("2f2a89fd-bcb9-4d5b-9fd9-08dd0b8c954a"),
            Guid.Parse("38233651-957f-4669-9f58-08dd0b8c954a"),
            Guid.Parse("a52ceea1-9346-4ec9-9fa8-08dd0b8c954a"),
            Guid.Parse("47373d77-fa7a-4538-9fe8-08dd0b8c954a"),
            Guid.Parse("491e8cd1-dcdf-4395-9fe5-08dd0b8c954a"),
            Guid.Parse("cfa745f2-4a9a-463e-9fd4-08dd0b8c954a"),
            Guid.Parse("f95d289d-97f6-44e5-a050-08dd0b8c954a"),
            Guid.Parse("dd2a6974-3258-49fd-9fbd-08dd0b8c954a"),
            Guid.Parse("7f1e9ba3-84c7-412d-9f84-08dd0b8c954a"),
            Guid.Parse("3c425958-25e5-49c1-9fcc-08dd0b8c954a"),
            Guid.Parse("84c80c2b-4807-4823-9fb2-08dd0b8c954a"),
            Guid.Parse("26afb07f-819b-4ac9-9f6c-08dd0b8c954a"),
            Guid.Parse("9bed4305-393b-4329-9fa4-08dd0b8c954a")
        };
        
            var categoriesList = await _context.Categories.Where(c => !excludedCategoryIds.Contains(c.ID.Value)).ToListAsync();
            return _mapper.Map<List<CategoriesDomain>>(categoriesList);
        }
        catch (SqlException)
        {
            throw new SqlExceptionCustom
            {
                DomainError = new DomainError
                {
                    StatusCode = DomainError.StatusCodeEnum.InternalServerError,
                    Type = DomainError.ErrorTypeEnum.DatabaseConnection,
                    Detail = "Looks like the database is temporarily unavailable.",
                    Field = DomainError.FieldTypeEnum.NotApplicable
                }
            };
        }
    }
    
    private async Task<List<OpenAIPlacesList>> GetPlacesWithinBoundingBoxAsync(
        float initialLatitude, 
        float initialLongitude, 
        float destinationLatitude, 
        float destinationLongitude,
        DomainUserPreferences userPreferences)
    {
        float minLatitude = Math.Min(initialLatitude, destinationLatitude);
        float maxLatitude = Math.Max(initialLatitude, destinationLatitude);
        float minLongitude = Math.Min(initialLongitude, destinationLongitude);
        float maxLongitude = Math.Max(initialLongitude, destinationLongitude);
        
        var excludedCategoryIds = new List<Guid>
        {
            Guid.Parse("343d5662-3323-4d00-a03a-08dd0b8c954a"), // American Restaurant
            Guid.Parse("2948ff8d-0019-4367-a009-08dd0b8c954a"), // Asian Restaurant
            Guid.Parse("77fc94be-e8dd-4c5a-9f5a-08dd0b8c954a"), // Bagel Shop
            Guid.Parse("347a1ec4-055f-4da9-9f29-08dd0b8c954a"), // Bakery
            Guid.Parse("ff7ffce7-d046-4bc3-9f12-08dd0b8c954a"), // Bar
            Guid.Parse("509b9d40-61c7-4bad-9f6e-08dd0b8c954a"), // BBQ Joint
            Guid.Parse("9b5661b6-b844-42ae-9f31-08dd0b8c954a"), // Cafe
            Guid.Parse("2b2e16ec-8ba3-47fc-9f26-08dd0b8c954a"), // Coffee Shop
            Guid.Parse("df7338ba-3077-46a1-9f14-08dd0b8c954a"), // Deli
            Guid.Parse("78077e71-07ae-4f73-9f0b-08dd0b8c954a"), // Dessert Shop
            Guid.Parse("2357bcee-2be6-42d4-9f15-08dd0b8c954a"), // Fast Food Restaurant
            Guid.Parse("60438c3a-f0fa-4b08-9f32-08dd0b8c954a"), // Food Truck
            Guid.Parse("e93f4633-9b99-47a3-9f18-08dd0b8c954a"), // French Restaurant
            Guid.Parse("13f68e5f-6e40-43f4-9f0e-08dd0b8c954a"), // Greek Restaurant
            Guid.Parse("ae91b3a4-4ab8-45f4-9f25-08dd0b8c954a"), // Indian Restaurant
            Guid.Parse("518b7ef9-c0a2-43f6-9f19-08dd0b8c954a"), // Italian Restaurant
            Guid.Parse("6d6508b5-4f42-4e72-9f2a-08dd0b8c954a"), // Japanese Restaurant
            Guid.Parse("7b9d4e5b-b8d7-4951-9f22-08dd0b8c954a"), // Mexican Restaurant
            Guid.Parse("4d8b5b17-776e-48f4-9f27-08dd0b8c954a"), // Pizza Place
            Guid.Parse("3a15c0dc-761f-4c23-9f23-08dd0b8c954a"), // Seafood Restaurant
            Guid.Parse("4e286733-8cbf-4e3d-9f10-08dd0b8c954a"), // Steakhouse
            Guid.Parse("02ac487f-d0de-47ec-9f2b-08dd0b8c954a"), // Sushi Restaurant
            Guid.Parse("7cd8c6f8-f340-4c94-9f17-08dd0b8c954a"), // Thai Restaurant
            Guid.Parse("88ef4605-0f7f-46b2-9f28-08dd0b8c954a"), // Vegan Restaurant
            Guid.Parse("dcd1a5b0-2e1b-4590-9f24-08dd0b8c954a"), // Vegetarian Restaurant
            Guid.Parse("a3bbf6d2-94f1-41a0-9f16-08dd0b8c954a"),  // Vietnamese Restaurant
            Guid.Parse("bfcaaf24-48f9-41a3-9f40-08dd0b8c954a"), //pt restaurant
            Guid.Parse("838ff12e-989a-4a49-9f60-08dd0b8c954a"), //night club
            Guid.Parse("d79ec876-1e88-413e-9faf-08dd0b8c954a"), //pub
            Guid.Parse("91834716-28bf-4d86-9fa9-08dd0b8c954a"),
            Guid.Parse("b2901a3a-4fd0-49d6-9f87-08dd0b8c954a"),
            Guid.Parse("508746f3-0db8-41fb-9f14-08dd0b8c954a"),
            Guid.Parse("595af998-f29f-4fa2-9f42-08dd0b8c954a"),
            Guid.Parse("394dfdd7-3afd-49d7-9fe1-08dd0b8c954a"), //chinese restaurant
            Guid.Parse("e0c9cc26-c074-43a1-9f27-08dd0b8c954a"), //restaurant
            Guid.Parse("400d03ad-cc7b-46bf-9fc1-08dd0b8c954a"), //bistro
            Guid.Parse("e74321c2-d3f7-4514-9f6f-08dd0b8c954a"), //pizzaria
            Guid.Parse("a505a34a-ac21-40bb-9f43-08dd0b8c954a"), //tapas
            Guid.Parse("e5153fb2-7226-4d92-9f83-08dd0b8c954a"), //brewery
            Guid.Parse("c3ed42aa-7577-4327-9f6b-08dd0b8c954a"), //italian restaurant
            Guid.Parse("5c5b0a9a-7209-4009-9fa7-08dd0b8c954a"), // gastro pub
            Guid.Parse("d5c30740-351a-4866-9faa-08dd0b8c954a"), //auto service ?????
            Guid.Parse("13aef974-3b67-4c5d-9f63-08dd0b8c954a"), //agro service
            
            
            
        };

        var places = await _context.Places
            .Include(p => p.Categories)
            .Where(p => p.Latitude >= minLatitude && p.Latitude <= maxLatitude &&
                        p.Longitude >= minLongitude && p.Longitude <= maxLongitude &&
                        !p.Categories.Any(c => c.ID.HasValue && excludedCategoryIds.Contains(c.ID.Value)))
            .ToListAsync();
        
        var mandatoryPlaces = await _context.Places
            .Where(p => userPreferences.MandatoryToVisit.Any(pattern => EF.Functions.Like(p.Name, pattern))) 
            .ToListAsync();

        mandatoryPlaces.ForEach(mtv => places.Add(mtv));
        userPreferences.MandatoryToVisit.Clear();
        mandatoryPlaces.ForEach(p => userPreferences.MandatoryToVisit.Add(p.Id.ToString()));
        
        var placeReturn = places.Select(place => new OpenAIPlacesList { Id = place.Id, Longitude = place.Longitude, Latitude = place.Latitude }).ToList();

        return placeReturn;
    }

    public async Task<List<List<string>>> GetOpenAIRoadTripPlan(DomainUserPreferences userPreferences)
    {
        var openAIRequest = new OpenAIRequest()
        {
            Places = [],
            UserPreferences = _mapper.Map<OpenAIUserPreferences>(userPreferences)
        };
        
        openAIRequest.Places = await GetPlacesWithinBoundingBoxAsync(
            userPreferences.StartingLat,
            userPreferences.StartingLon, 
            userPreferences.DestinationLat,
            userPreferences.DestinationLon,
            userPreferences);
        
       var requestBody = new
        {
            model = "gpt-4o-mini",
            messages = new OpenAIRoleMessage[]
            {
                new()
                {
                    role = "system",
                    content = "Plan a road trip with ordered POIs between a start and end point, using preferences and coordinates for efficient routing, adding places on the return if BackHome is true, and if the mandatoryToVisit contains ID's that are in the provided place list, you MUST add them to ALL TRIPS. Ensure trips are 3+ hours, dividing into trips with 4-10 unique places each, and returning ONLY JSON with POI IDs: { \"results\": [ { \"trip1\": [ { \"id\": \"poi1\" }, { \"id\": \"poi2\" } ] }, { \"trip2\": [ { \"id\": \"poi3\" }, { \"id\": \"poi4\" } ] } ], \"totalTrips\": x }",
                },
                new()
                {
                    role = "user",
                    content = JsonConvert.SerializeObject(new 
                    { 
                        places = openAIRequest.Places, 
                        userPreferences = openAIRequest.UserPreferences 
                    }) 
                }
            }
        };

        var jsonRequestBody = JsonConvert.SerializeObject(requestBody);

        using (var client = new HttpClient())
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _openAiKey);
            var content = new StringContent(jsonRequestBody, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(_openAiUrl, content);

            var openAIResponseJSON = await response.Content.ReadAsStringAsync();
            var openAIResponseParsed = JsonConvert.DeserializeObject<OpenAIResponse>(openAIResponseJSON);
            var tripResultsFinal = openAIResponseParsed.Choices[0].Message.Content;
            var cleanedContent = tripResultsFinal.Trim();
            cleanedContent = cleanedContent.Trim('`');
            if (cleanedContent.StartsWith("json", StringComparison.OrdinalIgnoreCase))
            {
                cleanedContent = cleanedContent.Substring(4).Trim(); 
            }
            
            var parsedJson = JObject.Parse(cleanedContent);
            var totalTrips = parsedJson["totalTrips"];
            var trips = new List<List<string>>();  // List of lists to store the trips

            // Iterate through each trip in the "results" array
            for (int i = 0; i < totalTrips.Value<int>(); i++)
            {
                var tripKey = $"trip{i + 1}";

                var tripArray = parsedJson["results"]?[i][tripKey];

                if (tripArray != null)
                {
                    var tripIds = new List<string>();

                    foreach (var trip in tripArray)
                    {
                        var id = trip["id"]?.ToString();  
                        if (id != null)
                        {
                            tripIds.Add(id);  
                        }
                    }
                    
                    trips.Add(tripIds);
                }
            }

            return trips;
        }
    }

    private async Task<bool> VerifyDuplicatePlace(PlaceDB place)
    {
        return await _context.Places.AnyAsync(p => p.Name == place.Name);
    }
    
}
