using System.Collections.Immutable;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TripifyBackend.DOMAIN.Interfaces.Repository;
using TripifyBackend.DOMAIN.Interfaces.Service;
using TripifyBackend.DOMAIN.Models;
using Results = TripifyBackend.DOMAIN.Models.Results;

namespace TripifyBackend.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoutesController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IRepository _repo;
        private readonly IService _service;
        
        public RoutesController(IMapper mapper, IRepository repo, IService service)
        {
            _mapper = mapper;
            _repo = repo;
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> FillDatabaseController(string lat, string lon)
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

                var placesDomain = _mapper.Map<List<PlaceDomain>>(place.results);
                var list = await _repo.FillDatabase(placesDomain);
                
                return Ok(placesDomain);
            }
            else
            {
                return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
            }
        }

        [HttpGet]
        [Route("tripRoute")]
        public async Task<IActionResult> GetTripRoute([FromQuery] string lat, [FromQuery] string lon, 
            [FromQuery] int tripDuration, 
            [FromQuery] double maxDistance, 
            [FromQuery] List<string> categories, 
            [FromQuery] bool includeHotel,
            [FromQuery] string travelMode,
            [FromQuery] string targetGroup,
            [FromQuery] List<string> mandatoryToVisit,
            [FromQuery] string? budget
            )
        {
            var tripLocations = await _service.GetTripRoute(lat, lon, tripDuration, maxDistance, categories, includeHotel, travelMode, targetGroup, mandatoryToVisit, budget);
            if (_service.GetErrors().Any())
            {
                return await ErrorHandler.HandleErrorAsync(_service.GetErrors().First());
            }

            return Ok();
        }

    }
}