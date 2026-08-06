using Microsoft.EntityFrameworkCore;

using RestaurantReservation.Infrastructure.Persistence;

namespace RestaurantReservation.UnitTests;

public static class TestDbContextFactory
{
    public static ApplicationDbContext Create()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.CreateVersion7().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }
}