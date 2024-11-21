using AutoMapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using TripifyBackend.DOMAIN.Interfaces.Repository;
using TripifyBackend.DOMAIN.Models;
using TripifyBackend.INFRA.DBContext;
using TripifyBackend.INFRA.Entities;
using TripifyBackend.INFRA.RepositoryExceptions;

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
        var addedPlaces = new List<PlaceDB>();
        foreach (var placeDomain in places)
        {
            var db = _mapper.Map<PlaceDB>(placeDomain);
            if (await VerifyDuplicatePlace(db) == false)
            {
                await _context.Places.AddAsync(db);
                addedPlaces.Add(db);
            }
        }

        
        await _context.SaveChangesAsync();
        return places;
    }

    public async Task<List<CategoriesDomain>> GetAllCategories()
    {
        try
        {
            var categoriesList = await _context.Categories.ToListAsync();
            
            return _mapper.Map<List<CategoriesDomain>>(categoriesList);
        }
        catch (SqlException e)
        {
            throw new SqlExceptionCustom
            {
                DomainError = new DomainError
                {
                    StatusCode = DomainError.StatusCodeEnum.InternalServerError,
                    Type = DomainError.ErrorTypeEnum.DatabaseConnection,
                    Detail = "Looks like database is temporarily unavailable.",
                    Field = DomainError.FieldTypeEnum.NotApplicable
                },
                
            };
        }
    }

    private async Task<bool> VerifyDuplicatePlace(PlaceDB place)
    {
        var placeFind = await _context.Places
            .FirstOrDefaultAsync(p => p.Name.Equals(place.Name));
        
        return placeFind != null ? true : false;
    }
}