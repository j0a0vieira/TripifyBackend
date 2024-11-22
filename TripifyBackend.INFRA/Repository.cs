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

                if (!await VerifyDuplicatePlace(db))
                {
                    await _context.Places.AddAsync(db);
                    addedPlaces.Add(db);
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
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

    private async Task<bool> VerifyDuplicatePlace(PlaceDB place)
    {
        return await _context.Places.AnyAsync(p => p.Name == place.Name);
    }
}
