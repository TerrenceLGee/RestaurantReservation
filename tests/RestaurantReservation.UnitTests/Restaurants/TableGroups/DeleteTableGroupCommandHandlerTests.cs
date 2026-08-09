using FluentAssertions;

using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

using NSubstitute;

using RestaurantReservation.Application.Features.Restaurants.TableGroups.Command.Delete;
using RestaurantReservation.Domain.Common;
using RestaurantReservation.Domain.Restaurants;
using RestaurantReservation.Domain.Tables;
using RestaurantReservation.UnitTests.Resources;

namespace RestaurantReservation.UnitTests.Restaurants.TableGroups;

public class DeleteTableGroupCommandHandlerTests
{
    private readonly HybridCache _cache = Substitute.For<HybridCache>();
    private readonly ILogger<DeleteTableGroupCommandHandler> _logger = Substitute.For<ILogger<DeleteTableGroupCommandHandler>>();

    [Fact]
    public async Task DeleteTableGroupCommandHandler_Should_Return_SuccessResult_OnSuccess()
    {
        await using var context = TestDbContextFactory.Create();
        const string groupName = "C# Programmers";

        (Restaurant restaurant, TableGroup tableGroup, List<Table> tables) =
            RestaurantResources.GetTableGroupToAdd(
                groupName,
                45,
                4);

        await context.Restaurants.AddAsync(restaurant, TestContext.Current.CancellationToken);
        await context.TableGroups.AddAsync(tableGroup, TestContext.Current.CancellationToken);
        await context.Tables.AddRangeAsync(tables, TestContext.Current.CancellationToken);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var command = new DeleteTableGroupCommand(restaurant.Id, tableGroup.Id);

        var handler = new DeleteTableGroupCommandHandler(context, _cache, _logger);

        var result = await handler.Handle(command, TestContext.Current.CancellationToken);

        result.IsSuccess.Should().BeTrue();
        result.Error.ErrorType.Should().Be(ErrorType.None);
    }

    [Fact]
    public async Task
        DeleteTableGroupCommandHandler_Should_Return_FailureResult_When_Attempting_To_Delete_NonExistent_TableGroup()
    {
        await using var context = TestDbContextFactory.Create();

        var addRestaurantCommand = RestaurantResources.AddRestaurant();
        var restaurant = RestaurantResources.GetRestaurantToAdd(addRestaurantCommand);

        await context.Restaurants.AddAsync(restaurant, TestContext.Current.CancellationToken);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var tableGroupId = Guid.CreateVersion7();

        var command = new DeleteTableGroupCommand(restaurant.Id, tableGroupId);

        var handler = new DeleteTableGroupCommandHandler(context, _cache, _logger);

        var result = await handler.Handle(command, TestContext.Current.CancellationToken);

        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("TableGroup.NotFound");
        result.Error.ErrorType.Should().Be(ErrorType.NotFound);
    }
}