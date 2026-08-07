using FluentAssertions;

using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

using NSubstitute;

using RestaurantReservation.Application.Features.Restaurants.Tables.Command.Delete;
using RestaurantReservation.Domain.Common;
using RestaurantReservation.Domain.Restaurants;
using RestaurantReservation.Domain.Tables;
using RestaurantReservation.UnitTests.Resources;

namespace RestaurantReservation.UnitTests.Restaurants.Tables;

public class DeleteTableCommandHandlerTests
{
    private readonly HybridCache _cache = Substitute.For<HybridCache>();
    private readonly ILogger<DeleteTableCommandHandler> _logger = Substitute.For<ILogger<DeleteTableCommandHandler>>();

    [Fact]
    public async Task DeleteTableCommandHandler_Returns_SuccessfulResult_When_Deleting_Table_OnSuccess()
    {
        await using var context = TestDbContextFactory.Create();

        (Restaurant restaurant, Table table) = RestaurantResources.GetTableToAdd(45);

        await context.Restaurants.AddAsync(restaurant, TestContext.Current.CancellationToken);
        await context.Tables.AddAsync(table, TestContext.Current.CancellationToken);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var command = new DeleteTableCommand(restaurant.Id, table.Id);

        var handler = new DeleteTableCommandHandler(context, _cache, _logger);

        var result = await handler.Handle(command, TestContext.Current.CancellationToken);

        result.IsSuccess.Should().BeTrue();
        result.Error.ErrorType.Should().Be(ErrorType.None);
    }

    [Fact]
    public async Task DeleteTableCommandHandler_Returns_FailureResult_When_Deleting_NonExistent_Table()
    {
        await using var context = TestDbContextFactory.Create();

        var addRestaurantCommand = RestaurantResources.AddRestaurant();
        
        var restaurant = RestaurantResources.GetRestaurantToAdd(addRestaurantCommand);

        await context.Restaurants.AddAsync(restaurant, TestContext.Current.CancellationToken);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var tableId = Guid.CreateVersion7();

        var command = new DeleteTableCommand(restaurant.Id, tableId);

        var handler = new DeleteTableCommandHandler(context, _cache, _logger);

        var result = await handler.Handle(command, TestContext.Current.CancellationToken);

        result.IsFailure.Should().BeTrue();
        result.Error.ErrorType.Should().Be(ErrorType.NotFound);
        result.Error.Code.Should().Be("Table.NotFound");
    }
}