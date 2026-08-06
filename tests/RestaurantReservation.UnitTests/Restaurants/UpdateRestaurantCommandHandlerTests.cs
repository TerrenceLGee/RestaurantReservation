using System.Runtime.InteropServices.JavaScript;

using FluentAssertions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

using NSubstitute;

using RestaurantReservation.Application.Features.Restaurants.Command.Update;
using RestaurantReservation.Domain.Common;
using RestaurantReservation.Domain.Restaurants;
using RestaurantReservation.UnitTests.Resources;

namespace RestaurantReservation.UnitTests.Restaurants;

public class UpdateRestaurantCommandHandlerTests
{
    private readonly HybridCache _cache = Substitute.For<HybridCache>();

    private readonly ILogger<UpdateRestaurantCommandHandler> _logger =
        Substitute.For<ILogger<UpdateRestaurantCommandHandler>>();

    [Fact]
    public async Task UpdateRestaurantCommandHandler_Should_Return_Result_Success_When_Successful()
    {
        var addRestaurantCommand = RestaurantResources.AddRestaurant();

        var restaurant = RestaurantResources.GetRestaurantToAdd(addRestaurantCommand);

        await using var context = TestDbContextFactory.Create();

        await context.Restaurants.AddAsync(restaurant, TestContext.Current.CancellationToken);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);
        
        var schedule = new RestaurantSchedule[]
        {
            new(WeekDay.Sunday, [new TimeOnly(9, 0), new TimeOnly(21, 00)]),
            new(WeekDay.Monday, [new TimeOnly(9, 0), new TimeOnly(21, 00)]),
            new(WeekDay.Tuesday, [new TimeOnly(9, 0), new TimeOnly(21, 00)]),
            new(WeekDay.Wednesday, [new TimeOnly(9, 0), new TimeOnly(21, 00)]),
            new(WeekDay.Thursday, [new TimeOnly(9, 0), new TimeOnly(22, 0)]),
            new(WeekDay.Friday, [new TimeOnly(9, 0), new TimeOnly(22, 30)]),
            new(WeekDay.Saturday, [new TimeOnly(9, 0), new TimeOnly(23, 00)])
        };

        var command = new UpdateRestaurantCommand(
            restaurant.Id,
            null,
            schedule);

        var handler = new UpdateRestaurantCommandHandler(context, _cache, _logger);

        var result = await handler.Handle(command, TestContext.Current.CancellationToken);

        var updatedRestaurant = await context.Restaurants
            .FirstOrDefaultAsync(r => r.Id == restaurant.Id, TestContext.Current.CancellationToken);

        result.IsSuccess.Should().BeTrue();
        result.Error.ErrorType.Should().Be(ErrorType.None);
        updatedRestaurant.Should().NotBeNull();
        updatedRestaurant.Id.Should().Be(restaurant.Id);
        updatedRestaurant.Schedule.ElementAt(0).DailyHours[0].Should().Be(new TimeOnly(9, 0));
        updatedRestaurant.Name.Should().Be("International House Of Culinary Delights");
    }
}