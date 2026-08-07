using FluentAssertions;

using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

using NSubstitute;

using RestaurantReservation.Application.Features.Restaurants.Command.Delete;
using RestaurantReservation.Domain.Common;
using RestaurantReservation.UnitTests.Resources;

namespace RestaurantReservation.UnitTests.Restaurants;

public class DeleteRestaurantCommandHandlerTests
{
    private readonly HybridCache _cache = Substitute.For<HybridCache>();

    private readonly ILogger<DeleteRestaurantCommandHandler> _logger =
        Substitute.For<ILogger<DeleteRestaurantCommandHandler>>();

    [Fact]
    public async Task DeleteRestaurantCommandHandler_Should_Return_SuccessResult_OnSuccess()
    {
        var addRestaurantCommand = RestaurantResources.AddRestaurant();

        var restaurant = RestaurantResources.GetRestaurantToAdd(addRestaurantCommand);

        await using var context = TestDbContextFactory.Create();

        await context.Restaurants.AddAsync(restaurant, TestContext.Current.CancellationToken);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var command = new DeleteRestaurantCommand(restaurant.Id);

        var handler = new DeleteRestaurantCommandHandler(context, _cache, _logger);

        var result = await handler.Handle(command, TestContext.Current.CancellationToken);

        result.IsSuccess.Should().BeTrue();
        result.Error.ErrorType.Should().Be(ErrorType.None);
    }

    [Fact]
    public async Task DeleteRestaurantCommandHandler_Should_Return_FailureResult_When_Restaurant_Is_NonExistent()
    {
        var restaurantId = Guid.CreateVersion7();

        await using var context = TestDbContextFactory.Create();

        var command = new DeleteRestaurantCommand(restaurantId);

        var handler = new DeleteRestaurantCommandHandler(context, _cache, _logger);

        var result = await handler.Handle(command, TestContext.Current.CancellationToken);

        result.IsSuccess.Should().BeFalse();
        result.Error.ErrorType.Should().Be(ErrorType.NotFound);
        result.Error.Code.Should().Be("Restaurant.NotFound");
    }
}