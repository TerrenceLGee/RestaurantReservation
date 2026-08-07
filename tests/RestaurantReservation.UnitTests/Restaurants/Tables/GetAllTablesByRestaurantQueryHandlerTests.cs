using System.Diagnostics;

using FluentAssertions;

using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

using NSubstitute;

using RestaurantReservation.Application.Features.Restaurants.Tables.Query.GetAll;
using RestaurantReservation.Application.Features.Restaurants.Tables.Query.Responses;
using RestaurantReservation.UnitTests.Resources;

namespace RestaurantReservation.UnitTests.Restaurants.Tables;

public class GetAllTablesByRestaurantQueryHandlerTests
{
    private readonly HybridCache _cache = Substitute.For<HybridCache>();

    private readonly ILogger<GetAllTablesByRestaurantQueryHandler> _logger =
        Substitute.For<ILogger<GetAllTablesByRestaurantQueryHandler>>();

    public GetAllTablesByRestaurantQueryHandlerTests()
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
    public async Task GetAllTablesByRestaurant_Returns_Result_Of_PagedResult_Page_2_PageSize_12_TableResponse_OnSuccess()
    {
        await using var context = TestDbContextFactory.Create();

        var addRestaurantCommand = RestaurantResources.AddRestaurant();
        var restaurant = RestaurantResources.GetRestaurantToAdd(addRestaurantCommand);

        await context.Restaurants.AddAsync(restaurant, TestContext.Current.CancellationToken);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var command = new GetAllTablesByRestaurantQuery(
            restaurant.Id,
            2,
            12);

        var handler = new GetAllTablesByRestaurantQueryHandler(context, _cache, _logger);
        var result = await handler.Handle(command, TestContext.Current.CancellationToken);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.TotalCount.Should().Be(132);
        result.Value.TotalPages.Should().Be(11);
        result.Value.HasNextPage.Should().BeTrue();
        result.Value.HasPreviousPage.Should().BeTrue();
        result.Value.Items.Count.Should().Be(12);
    }
}