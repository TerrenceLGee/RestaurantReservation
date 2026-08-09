using FluentAssertions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

using NSubstitute;

using RestaurantReservation.Application.Features.Restaurants.TableGroups.Command.Update;
using RestaurantReservation.Domain.Common;
using RestaurantReservation.Domain.Restaurants;
using RestaurantReservation.Domain.Tables;
using RestaurantReservation.UnitTests.Resources;

namespace RestaurantReservation.UnitTests.Restaurants.TableGroups;

public class UpdateTableGroupCommandHandlerTests
{
    private readonly HybridCache _cache = Substitute.For<HybridCache>();

    private readonly ILogger<UpdateTableGroupCommandHandler> _logger =
        Substitute.For<ILogger<UpdateTableGroupCommandHandler>>();

    [Fact]
    public async Task UpdateTableGroupCommandHandler_Should_Return_SuccessResult_OnSuccess()
    {
        await using var context = TestDbContextFactory.Create();
        const string groupName = "C# Programmers";
        const string updatedGroupName = "C# Developers";

        (Restaurant restaurant, TableGroup tableGroup, List<Table> tables) =
            RestaurantResources.GetTableGroupToAdd(
                groupName,
                45,
                4);

        await context.Restaurants.AddAsync(restaurant, TestContext.Current.CancellationToken);
        await context.TableGroups.AddAsync(tableGroup, TestContext.Current.CancellationToken);
        await context.Tables.AddRangeAsync(tables, TestContext.Current.CancellationToken);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var command = new UpdateTableGroupCommand(
            restaurant.Id,
            tableGroup.Id,
            updatedGroupName,
            30,
            4);

        var handler = new UpdateTableGroupCommandHandler(context, _cache, _logger);

        var result = await handler.Handle(command, TestContext.Current.CancellationToken);

        var updatedTableGroup = await context.TableGroups
            .Include(tg => tg.Tables)
            .FirstOrDefaultAsync(tg => tg.Id == tableGroup.Id && tg.RestaurantId == restaurant.Id,
                TestContext.Current.CancellationToken);

        result.IsSuccess.Should().BeTrue();
        updatedTableGroup.Should().NotBeNull();
        updatedTableGroup.Name.Should().Be(updatedGroupName);
        updatedTableGroup.Tables.Count.Should().Be(75);
    }

    [Fact]
    public async Task
        UpdateTableGroupCommandHandler_Should_Return_FailureResult_When_Trying_ToUpdate_TableGroup_Name_To_Already_Taken_GroupName()
    {
        await using var context = TestDbContextFactory.Create();
        const string groupName = "C# Programmers";
        const string updatedGroupName = "VIP Section";

        (Restaurant restaurant, TableGroup tableGroup, List<Table> tables) =
            RestaurantResources.GetTableGroupToAdd(
                groupName,
                45,
                4);

        await context.Restaurants.AddAsync(restaurant, TestContext.Current.CancellationToken);
        await context.TableGroups.AddAsync(tableGroup, TestContext.Current.CancellationToken);
        await context.Tables.AddRangeAsync(tables, TestContext.Current.CancellationToken);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var command = new UpdateTableGroupCommand(
            restaurant.Id,
            tableGroup.Id,
            updatedGroupName,
            30,
            4);

        var handler = new UpdateTableGroupCommandHandler(context, _cache, _logger);

        var result = await handler.Handle(command, TestContext.Current.CancellationToken);

        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("TableGroup.NameAlreadyTaken");
        result.Error.ErrorType.Should().Be(ErrorType.Conflict);
    }
}