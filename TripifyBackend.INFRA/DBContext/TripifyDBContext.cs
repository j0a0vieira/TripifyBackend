using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Primitives;
using TripifyBackend.INFRA.Entities;

namespace TripifyBackend.INFRA.DBContext;

public class TripifyDBContext : DbContext
{
    
    public TripifyDBContext(DbContextOptions<TripifyDBContext> options) : base(options)
    {
    }

    public DbSet<PlaceDB> Places { get; set; }
    public DbSet<CategoriesDB> Categories { get; set; }
    
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PlaceDB>()
            .HasMany(p => p.Categories)
            .WithMany(c => c.Places)
            .UsingEntity<Dictionary<string, object>>(
                "PlaceCategory", 
                pc => pc.HasOne<CategoriesDB>().WithMany().HasForeignKey("CategoryId"),
                pc => pc.HasOne<PlaceDB>().WithMany().HasForeignKey("PlaceId")
            );
    }
    
}