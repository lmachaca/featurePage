using Microsoft.EntityFrameworkCore;

namespace FeatureFlags.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options)
: DbContext(options)
{
    public DbSet<FeatureFlag> FeatureFlags => Set<FeatureFlag>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<FeatureFlag>().HasIndex(f => f.Name).IsUnique();

        
        b.Entity<FeatureFlag>().HasData(
        new FeatureFlag { Id = 1, Name = "Notificaciones", IsEnabled = true },
        new FeatureFlag { Id = 2, Name = "ModoOscuro", IsEnabled = false },
        new FeatureFlag { Id = 3, Name = "Ubicacion", IsEnabled = true });
    }
}