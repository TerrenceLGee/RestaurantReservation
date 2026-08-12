using FluentAssertions;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Npgsql;

using RestaurantReservation.Domain.Reservations;
using RestaurantReservation.Domain.Restaurants;
using RestaurantReservation.Domain.Users;

namespace RestaurantReservation.IntegrationTests.Reservations;

public class ReservationTableConstraintTests(IntegrationTestWebAppFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task TwoReservationsFor_TheSameTable_Should_Have_One_Succeed_And_OneFail()
    {
        using var userScope = Factory.Services.CreateScope();
        var userManager = userScope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        const string password = "Pa$$w0rd";
        
        var customer1 = new ApplicationUser
        {
            FirstName = "Dennis",
            LastName = "Edwards",
            Email = "customer1@example.com",
            UserName = "customer1@example.com",
            PhoneNumber = "555-111-2222",
            EmailConfirmed = true,
            RegistrationDate = DateOnly.FromDateTime(DateTime.Now)
        };
        await userManager.CreateAsync(customer1, password);

        var customer2 = new ApplicationUser
        {
            FirstName = "John",
            LastName = "Edwards",
            Email = "customer2@example.com",
            UserName = "customer2@example.com",
            PhoneNumber = "555-333-6655",
            EmailConfirmed = true,
            RegistrationDate = DateOnly.FromDateTime(DateTime.Now)
        };
        await userManager.CreateAsync(customer2, password);
        
        var restaurant = Restaurant.Create("Universal House Of Culinary Delights");
        const string groupName = "Outside Patio";

        var originalContext = CreateContext();

        var table = restaurant.AddTable(4);

        restaurant.AddTableGroup(groupName, [table]);

        await originalContext.Restaurants.AddAsync(restaurant, TestContext.Current.CancellationToken);
        await originalContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var context1 = CreateContext();
        var context2 = CreateContext();

        var reservation1 = Reservation.MakeReservation(
            restaurant.Id,
            restaurant.Name,
            customer1.Id,
            "Dennis",
            "Edwards",
            "customer1@example.com",
            "555-111-2222",
            new DateOnly(2026, 10, 1),
            new TimeOnly(16, 30),
            new TimeOnly(18, 30),
            3,
            groupName);
        
        var reservation2 = Reservation.MakeReservation(
            restaurant.Id,
            restaurant.Name,
            customer2.Id,
            "John",
            "Edwards",
            "customer2@example.com",
            "555-333-6655",
            new DateOnly(2026, 10, 1),
            new TimeOnly(16, 30),
            new TimeOnly(18, 30),
            3,
            groupName);

        reservation1.IsSuccess.Should().BeTrue();
        reservation2.IsSuccess.Should().BeTrue();

        var table1 =
            await context1.Tables.FirstOrDefaultAsync(t => t.Id == table.Id, TestContext.Current.CancellationToken);
        var table2 =
            await context2.Tables.FirstOrDefaultAsync(t => t.Id == table.Id, TestContext.Current.CancellationToken);

        table1.Should().NotBeNull();
        table2.Should().NotBeNull();

        var reservationTable1 = table1.ReserveTable(
            new DateOnly(2026, 10, 1),
            new TimeOnly(16, 30),
            new TimeOnly(18, 30));
        var reservationTable2 = table2.ReserveTable(
            new DateOnly(2026, 10, 1),
            new TimeOnly(16, 30),
            new TimeOnly(18, 30));

        reservationTable1.IsSuccess.Should().BeTrue();
        reservationTable2.IsSuccess.Should().BeTrue();

        reservationTable1.Value.ReservationId = reservation1.Value.Id;
        reservationTable2.Value.ReservationId = reservation2.Value.Id;
        
        reservation1.Value.AddReservationTable(reservationTable1.Value);
        reservation2.Value.AddReservationTable(reservationTable2.Value);

        await context1.Reservations.AddAsync(reservation1.Value, TestContext.Current.CancellationToken);
        await context2.Reservations.AddAsync(reservation2.Value, TestContext.Current.CancellationToken);

        var task1 = context1.SaveChangesAsync(TestContext.Current.CancellationToken);
        var task2 = context2.SaveChangesAsync(TestContext.Current.CancellationToken);

        Exception? failure1 = null, failure2 = null;

        try { await task1; } catch (DbUpdateException ex) { failure1 = ex; }

        try { await task2; } catch (DbUpdateException ex) { failure2 = ex; }

        var actualFailure = failure1 ?? failure2;

        await using var verificationContext = CreateContext();
        var count = await verificationContext.Set<ReservationTable>()
            .CountAsync(rt => rt.TableId == table.Id, TestContext.Current.CancellationToken);

        (failure1 is null ^ failure2 is null).Should().BeTrue();
        actualFailure!.InnerException.Should().BeOfType<PostgresException>()
            .Which.SqlState.Should().Be("23P01");
        count.Should().Be(1);
    }
}