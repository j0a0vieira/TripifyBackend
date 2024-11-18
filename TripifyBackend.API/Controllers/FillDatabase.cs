using System.Collections.Immutable;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TripifyBackend.DOMAIN.Interfaces.Repository;
using TripifyBackend.DOMAIN.Models;
using Results = TripifyBackend.DOMAIN.Models.Results;

namespace TripifyBackend.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FillDatabase : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IRepository _repo;
        
        public FillDatabase(IMapper mapper, IRepository repo)
        {
            _mapper = mapper;
            _repo = repo;
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
                
                
                
                return Ok(place);
            }
            else
            {
                return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
            }
        }

    }
}
