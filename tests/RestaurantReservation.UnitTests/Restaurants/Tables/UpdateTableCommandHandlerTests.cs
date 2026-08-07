using FluentAssertions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

using NSubstitute;

using RestaurantReservation.Application.Features.Restaurants.Tables.Command.Update;
using RestaurantReservation.Domain.Common;
using RestaurantReservation.Domain.Restaurants;
using RestaurantReservation.Domain.Tables;
using RestaurantReservation.UnitTests.Resources;

namespace RestaurantReservation.UnitTests.Restaurants.Tables;

public class UpdateTableCommandHandlerTests
{
    private readonly HybridCache _cache = Substitute.For<HybridCache>();
    private readonly ILogger<UpdateTableCommandHandler> _logger = Substitute.For<ILogger<UpdateTableCommandHandler>>();

    [Fact]
    public async Task UpdateTableCommandHandler_Returns_SuccessResult_When_UpdatingTable_To_Join_TableGroup_OnSuccess()
    {
        await using var context = TestDbContextFactory.Create();

        (Restaurant restaurant, Table table) = RestaurantResources.GetTableToAdd(45);

        await context.Restaurants.AddAsync(restaurant, TestContext.Current.CancellationToken);
        await context.Tables.AddAsync(table, TestContext.Current.CancellationToken);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var command = new UpdateTableCommand(
            restaurant.Id,
            table.Id,
            null,
            "VIP Section");

        var handler = new UpdateTableCommandHandler(context, _cache, _logger);

        var result = await handler.Handle(command, TestContext.Current.CancellationToken);

        var updatedTable = await context.Tables
            .FirstOrDefaultAsync(t => t.RestaurantId == restaurant.Id && t.Id == table.Id,
                TestContext.Current.CancellationToken);

        result.IsSuccess.Should().BeTrue();
        updatedTable.Should().NotBeNull();
        updatedTable.SeatsAtTable.Should().Be(45);
        updatedTable.IsInTableGroup.Should().BeTrue();
        updatedTable.TableGroupName.Should().Be("VIP Section");
    }

    [Fact]
    public async Task
        UpdateTableCommandHandler_Returns_FailureResult_When_TryingTo_Update_Table_To_NonExistent_TableGroup()
    {
        await using var context = TestDbContextFactory.Create();

        (Restaurant restaurant, Table table) = RestaurantResources.GetTableToAdd(45);

        await context.Restaurants.AddAsync(restaurant, TestContext.Current.CancellationToken);
        await context.Tables.AddAsync(table, TestContext.Current.CancellationToken);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var command = new UpdateTableCommand(
            restaurant.Id,
            table.Id,
            null,
            "Programmer's Dining Room");

        var handler = new UpdateTableCommandHandler(context, _cache, _logger);

        var result = await handler.Handle(command, TestContext.Current.CancellationToken);

        result.IsFailure.Should().BeTrue();
        result.Error.ErrorType.Should().Be(ErrorType.NotFound);
        result.Error.Code.Should().Be("TableGroup.NotFound");
    }

    [Fact]
    public async Task UpdateTableCommandHandler_Returns_FailureResult_When_Trying_To_Update_Table_To_Same_TableGroup()
    {
        await using var context = TestDbContextFactory.Create();

        const string groupName = "Outside Patio";

        (Restaurant restaurant, Table table) = RestaurantResources.GetTableToAdd(45, groupName);

        await context.Restaurants.AddAsync(restaurant, TestContext.Current.CancellationToken);
        await context.Tables.AddAsync(table, TestContext.Current.CancellationToken);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var command = new UpdateTableCommand(
            restaurant.Id,
            table.Id,
            null,
            groupName);

        var handler = new UpdateTableCommandHandler(context, _cache, _logger);

        var result = await handler.Handle(command, TestContext.Current.CancellationToken);

        result.IsFailure.Should().BeTrue();
        result.Error.ErrorType.Should().Be(ErrorType.Conflict);
        result.Error.Code.Should().Be("Table.AlreadyInTableGroup");
    }
}