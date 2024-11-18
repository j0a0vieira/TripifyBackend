using AutoMapper;
using TripifyBackend.DOMAIN.Interfaces.Repository;
using TripifyBackend.DOMAIN.Models;
using TripifyBackend.INFRA.DBContext;
using TripifyBackend.INFRA.Entities;

namespace TripifyBackend.INFRA;

public class Repository : IRepository
{
    private readonly TripifyDBContext _context;
    private readonly IMapper _mapper;
    
    public Repository(TripifyDBContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    
    public async Task<List<PlaceDomain>> FillDatabase(List<PlaceDomain> places)
    {
        foreach (var placeDomain in places)
        {
            var db = _mapper.Map<PlaceDB>(placeDomain);
            await _context.Places.AddAsync(db);
        }

        
        await _context.SaveChangesAsync();
        return places;
    }
}