using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using RestaurantReservation.Application.Abstractions;
using RestaurantReservation.Domain.Reservations;
using RestaurantReservation.Domain.Restaurants;
using RestaurantReservation.Domain.Tables;
using RestaurantReservation.Domain.Users;
using RestaurantReservation.Infrastructure.Persistence.Seeding;

namespace RestaurantReservation.Infrastructure.Persistence;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
    : IdentityDbContext<ApplicationUser, IdentityRole, string>(options), IApplicationDbContext
{
    public DbSet<Restaurant> Restaurants => Set<Restaurant>();
    public DbSet<Table> Tables => Set<Table>();
    public DbSet<TableGroup> TableGroups => Set<TableGroup>();
    public DbSet<Reservation> Reservations => Set<Reservation>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<ReservationTable> ReservationTables => Set<ReservationTable>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseAsyncSeeding(async (context, _, cancellationToken) =>
            {
                if (!await context.Set<Restaurant>().AnyAsync(cancellationToken))
                {
                    var restaurants = SeedingResources.GetRestaurantsForSeeding();
                    await context.Set<Restaurant>().AddRangeAsync(restaurants, cancellationToken);
                    await context.SaveChangesAsync(cancellationToken);
                }
            })
            .UseSeeding((context, _) =>
            {
                if (!context.Set<Restaurant>().Any())
                {
                    var restaurants = SeedingResources.GetRestaurantsForSeeding();
                    context.Set<Restaurant>().AddRange(restaurants);
                    context.SaveChanges();
                }
            });
    }
}