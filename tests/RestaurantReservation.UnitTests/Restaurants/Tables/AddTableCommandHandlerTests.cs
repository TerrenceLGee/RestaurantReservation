using FluentAssertions;

using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

using NSubstitute;

using RestaurantReservation.Application.Features.Restaurants.Tables.Command.Add;
using RestaurantReservation.Domain.Common;
using RestaurantReservation.Infrastructure.Persistence;
using RestaurantReservation.UnitTests.Resources;

namespace RestaurantReservation.UnitTests.Restaurants.Tables;

public class AddTableCommandHandlerTests
{
    private readonly HybridCache _cache = Substitute.For<HybridCache>();
    private readonly ILogger<AddTableCommandHandler> _logger = Substitute.For<ILogger<AddTableCommandHandler>>();

    [Fact]
    public async Task AddTableCommandHandler_Should_Return_Result_Of_RestaurantTableResponse_OnSuccess()
    {
        await using var context = TestDbContextFactory.Create();

        var restaurantId = await AddRestaurantForTable(context);

        var command = new AddTableCommand(
            restaurantId,
            40,
            "Main Dining Room");

        var handler = new AddTableCommandHandler(context, _cache, _logger);

        var result = await handler.Handle(command, TestContext.Current.CancellationToken);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.SeatsAtTable.Should().Be(40);
        result.Value.TableGroupName.Should().Be("Main Dining Room");
    }

    [Fact]
    public async Task
        AddTableCommandHandler_Should_Return_FailureResult_WhenTryingToAddTable_To_NonExistent_Restaurant()
    {
        await using var context = TestDbContextFactory.Create();

        var restaurantId = Guid.CreateVersion7();

        var command = new AddTableCommand(
            restaurantId,
            45,
            "Main Dining Room");

        var handler = new AddTableCommandHandler(context, _cache, _logger);

        var result = await handler.Handle(command, TestContext.Current.CancellationToken);

        result.IsFailure.Should().BeTrue();
        result.Error.ErrorType.Should().Be(ErrorType.NotFound);
        result.Error.Code.Should().Be("Restaurant.NotFound");
    }

    private static async Task<Guid> AddRestaurantForTable(ApplicationDbContext context)
    {
        var command = RestaurantResources.AddRestaurant();

        var restaurant = RestaurantResources.GetRestaurantToAdd(command);

        await context.Restaurants.AddAsync(restaurant);
        await context.SaveChangesAsync();

        return restaurant.Id;
    }
}