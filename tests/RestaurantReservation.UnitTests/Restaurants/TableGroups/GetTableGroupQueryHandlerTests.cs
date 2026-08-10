using System.Diagnostics;

using FluentAssertions;

using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

using NSubstitute;

using RestaurantReservation.Application.Features.Restaurants.TableGroups.Query.Get;
using RestaurantReservation.Application.Features.Restaurants.TableGroups.Query.Responses;
using RestaurantReservation.Domain.Common;
using RestaurantReservation.Domain.Restaurants;
using RestaurantReservation.Domain.Tables;
using RestaurantReservation.UnitTests.Resources;

namespace RestaurantReservation.UnitTests.Restaurants.TableGroups;

public class GetTableGroupQueryHandlerTests
{
    private readonly HybridCache _cache = Substitute.For<HybridCache>();
    private readonly ILogger<GetTableGroupQueryHandler> _logger = Substitute.For<ILogger<GetTableGroupQueryHandler>>();

    public GetTableGroupQueryHandlerTests()
    {
        _cache.GetOrCreateAsync(
                Arg.Any<string>(),
                Arg.Any<Func<CancellationToken, ValueTask<TableGroupDetailResponse>>>(),
                Arg.Any<HybridCacheEntryOptions>(),
                Arg.Any<IEnumerable<string>>(),
                Arg.Any<CancellationToken>())
            .Returns<ValueTask<TableGroupDetailResponse>>(async callInfo =>
            {
                var factory = callInfo.Arg<Func<CancellationToken, ValueTask<TableGroupDetailResponse>>>();
                var token = callInfo.Arg<CancellationToken>();
                Debug.Assert(factory != null, nameof(factory) + " != null");
                return await factory(token);
            });
    }

    [Fact]
    public async Task GetTableGroupQueryHandler_Returns_SuccessResult_Of_TableGroupDetailResponse_OnSuccess()
    {
        await using var context = TestDbContextFactory.Create();

        const string groupName = "C# Developers";

        (Restaurant restaurant, TableGroup tableGroup, List<Table> tables) = RestaurantResources.GetTableGroupToAdd(
            groupName,
            45,
            4);

        await context.Restaurants.AddAsync(restaurant, TestContext.Current.CancellationToken);
        await context.TableGroups.AddAsync(tableGroup, TestContext.Current.CancellationToken);
        await context.Tables.AddRangeAsync(tables, TestContext.Current.CancellationToken);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var query = new GetTableGroupQuery(restaurant.Id, tableGroup.Id);

        var handler = new GetTableGroupQueryHandler(context, _cache, _logger);

        var result = await handler.Handle(query, TestContext.Current.CancellationToken);

        result.IsSuccess.Should().BeTrue();
        result.Error.ErrorType.Should().Be(ErrorType.None);
        result.Value.Should().NotBeNull();
        result.Value.NumberOfTables.Should().Be(45);
        result.Value.GroupName.Should().Be(groupName);
        result.Value.RestaurantName.Should().Be(restaurant.Name);
        result.Value.RestaurantId.Should().Be(restaurant.Id);
        result.Value.Id.Should().Be(tableGroup.Id);
    }

    [Fact]
    public async Task
        GetTableGroupQueryHandler_Returns_FailureResult_When_Attempting_ToRetrieve_NonExistent_TableGroup()
    {
        await using var context = TestDbContextFactory.Create();

        var addRestaurantCommand = RestaurantResources.AddRestaurant();
        var restaurant = RestaurantResources.GetRestaurantToAdd(addRestaurantCommand);
        var tableGroupId = Guid.CreateVersion7();

        await context.Restaurants.AddAsync(restaurant, TestContext.Current.CancellationToken);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var query = new GetTableGroupQuery(restaurant.Id, tableGroupId);

        var handler = new GetTableGroupQueryHandler(context, _cache, _logger);

        var result = await handler.Handle(query, TestContext.Current.CancellationToken);

        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("TableGroup.NotFound");
        result.Error.ErrorType.Should().Be(ErrorType.NotFound);
    } 
}