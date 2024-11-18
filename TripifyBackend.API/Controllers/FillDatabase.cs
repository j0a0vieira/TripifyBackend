using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TripifyBackend.DOMAIN.Models;
using Results = TripifyBackend.DOMAIN.Models.Results;

namespace TripifyBackend.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FillDatabase : ControllerBase
    {
        public FillDatabase()
        {
            
        }

        [HttpGet]
        public async Task<IActionResult> FillDatabaseController()
        {
            using var http = new HttpClient
            {
                BaseAddress = new Uri("https://api.foursquare.com/v3/places/")
            };
            
            http.DefaultRequestHeaders.Clear(); // Opcional, mas pode ajudar a limpar outros cabeçalhos que possam interferir
            http.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "fsq3ua+/Y3GwtN4ciiOooUqdCGKnFLQmM4iFcp8o9In42M0=");

            var response = await http.GetAsync("search?ll=39.65859633425525,-8.828578528983424&radius=5000");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                Results? place = JsonSerializer.Deserialize<Results>(content);  
                
                foreach (var placeResult in place.results)
                {
                    placeResult.Id = Guid.NewGuid();
                }
                
                
                return Ok(place);
            }
            else
            {
                return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
            }
        }

    }
}
