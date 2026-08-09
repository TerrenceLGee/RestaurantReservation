using System.Diagnostics;

using FluentAssertions;

using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

using NSubstitute;

using RestaurantReservation.Application.Features.Restaurants.Tables.Query.GetAll;
using RestaurantReservation.Application.Features.Restaurants.Tables.Query.Responses;
using RestaurantReservation.Domain.Common;
using RestaurantReservation.UnitTests.Resources;

namespace RestaurantReservation.UnitTests.Restaurants.Tables;

public class GetAllTablesByTableGroupQueryHandlerTests
{
    private readonly HybridCache _cache = Substitute.For<HybridCache>();

    private readonly ILogger<GetAllTablesByTableGroupQueryHandler> _logger =
        Substitute.For<ILogger<GetAllTablesByTableGroupQueryHandler>>();

    public GetAllTablesByTableGroupQueryHandlerTests()
    {
        _cache.GetOrCreateAsync(
                Arg.Any<string>(),
                Arg.Any<Func<CancellationToken, ValueTask<(List<TableResponse> items, int totalCount)>>>(),
                Arg.Any<HybridCacheEntryOptions>(),
                Arg.Any<IEnumerable<string>>(),
                Arg.Any<CancellationToken>())
            .Returns<ValueTask<(List<TableResponse> items, int totalCount)>>(async callInfo =>
            {
                var factory = callInfo
                    .Arg<Func<CancellationToken, ValueTask<(List<TableResponse> items, int totalCount)>>>();
                var token = callInfo.Arg<CancellationToken>();
                Debug.Assert(factory != null, nameof(factory) + " != null");
                return await factory(token);
            });
    }

    [Fact]
    public async Task GetAllTablesByTableGroup_Returns_Result_Of_PagedResult_Page_2_PageSize_3_TableResponse_OnSuccess()
    {
        await using var context = TestDbContextFactory.Create();

        var addRestaurantCommand = RestaurantResources.AddRestaurant();
        var restaurant = RestaurantResources.GetRestaurantToAdd(addRestaurantCommand);

        await context.Restaurants.AddAsync(restaurant, TestContext.Current.CancellationToken);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        const string groupName = "Family Dining Room";

        var command = new GetAllTablesByTableGroupQuery(
            restaurant.Id, 
            groupName, 
            2, 
            3);

        var handler = new GetAllTablesByTableGroupQueryHandler(context, _cache, _logger);

        var result = await handler.Handle(command, TestContext.Current.CancellationToken);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.HasNextPage.Should().BeTrue();
        result.Value.HasPreviousPage.Should().BeTrue();
        result.Value.TotalCount.Should().Be(30);
        result.Value.TotalPages.Should().Be(10);
        result.Value.Items.Count.Should().Be(3);
        result.Error.ErrorType.Should().Be(ErrorType.None);
    }

    [Fact]
    public async Task GetAllTablesByTableGroup_Returns_FailureResult_When_TableGroup_Is_NonExistent()
    {
        await using var context = TestDbContextFactory.Create();

        var addRestaurantCommand = RestaurantResources.AddRestaurant();
        var restaurant = RestaurantResources.GetRestaurantToAdd(addRestaurantCommand);

        await context.Restaurants.AddAsync(restaurant, TestContext.Current.CancellationToken);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        const string groupName = "C# Developers";

        var command = new GetAllTablesByTableGroupQuery(restaurant.Id, groupName);

        var handler = new GetAllTablesByTableGroupQueryHandler(context, _cache, _logger);

        var result = await handler.Handle(command, TestContext.Current.CancellationToken);

        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("Tables.NotFound");
        result.Error.ErrorType.Should().Be(ErrorType.NotFound);
    }
}