using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TripifyBackend.DOMAIN.Interfaces.Repository;
using TripifyBackend.DOMAIN.Interfaces.Service;

namespace TripifyBackend.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FrontSupportController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IService _service;
        
        public FrontSupportController(IMapper mapper, IService service)
        {
            _mapper = mapper;
            _service = service;
        }
        
        //add a controller that returns "hello world"ionResult Get()"hello world";
        [HttpGet]
        [Route("categoryList")]
        public async Task<IActionResult> GetTripRoute()
        {
            var categories = await _service.GetAllCategories();
            if (_service.GetErrors().Count != 0)
            {
                return await ErrorHandler.HandleErrorAsync(_service.GetErrors().First());
            }

            return Ok(categories);
        }

        [HttpGet]
        [Route("fillDatabase")]
        public async Task<IActionResult> FillDatabase(string lat, string lon)
        {
            var addedPlaces = await _service.FillDatabase(lat, lon);

            return Ok(addedPlaces);
        }
    }
}