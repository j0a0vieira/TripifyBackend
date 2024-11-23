using System.Collections.Immutable;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TripifyBackend.API.DTO_s;
using TripifyBackend.DOMAIN.Interfaces.Repository;
using TripifyBackend.DOMAIN.Interfaces.Service;
using TripifyBackend.DOMAIN.Models;
using TripifyBackend.INFRA.Entities;
using Results = TripifyBackend.DOMAIN.Models.Results;

namespace TripifyBackend.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoutesController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IService _service;
        
        public RoutesController(IMapper mapper, IService service)
        {
            _mapper = mapper;
            _service = service;
        }

        [HttpPost]
        [Route("tripRoute")]
        public async Task<IActionResult> GetTripRoute([FromBody] GetTripRouteRequest userPreferences)
        {
            var tripLocations = await _service.GetTripRoute(_mapper.Map<DomainUserPreferences>(userPreferences));
            var response = new GetTripRouteResponse
            {
                Trips = _mapper.Map<List<TripDTO>>(tripLocations),
                StartDate = userPreferences.StartDate,
                EndDate = userPreferences.EndDate,
            };
            
            if (_service.GetErrors().Count != 0)
            {
                return await ErrorHandler.HandleErrorAsync(_service.GetErrors().First());
            }

            return Ok(response);
        }
    }
}