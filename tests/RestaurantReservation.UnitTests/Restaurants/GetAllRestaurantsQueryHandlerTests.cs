using System.Diagnostics;

using FluentAssertions;

using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

using NSubstitute;

using RestaurantReservation.Application.Features.Restaurants.Query.GetAll;
using RestaurantReservation.Application.Features.Restaurants.Query.Response;
using RestaurantReservation.UnitTests.Resources;

namespace RestaurantReservation.UnitTests.Restaurants;

public class GetAllRestaurantsQueryHandlerTests
{
    private readonly HybridCache _cache;
    private readonly ILogger<GetAllRestaurantsQueryHandler> _logger;

    public GetAllRestaurantsQueryHandlerTests()
    {
        _cache = Substitute.For<HybridCache>();
        _logger = Substitute.For<ILogger<GetAllRestaurantsQueryHandler>>();
        _cache.GetOrCreateAsync(
                Arg.Any<string>(),
                Arg.Any<Func<CancellationToken, ValueTask<(List<RestaurantResponse> items, int totalCount)>>>(),
                Arg.Any<HybridCacheEntryOptions>(),
                Arg.Any<IEnumerable<string>>(),
                Arg.Any<CancellationToken>())
            .Returns<ValueTask<(List<RestaurantResponse> items, int totalCount)>>(async callInfo =>
            {
                var factory = callInfo
                    .Arg<Func<CancellationToken, ValueTask<(List<RestaurantResponse> items, int totalCount)>>>();
                var token = callInfo.Arg<CancellationToken>();
                Debug.Assert(factory != null, nameof(factory) + " != null");
                return await factory(token);
            });
    }

    [Fact]
    public async Task
        GetAllRestaurants_Should_Return_Result_Of_PagedResult_Page_1_PageSize_3_RestaurantDetailResponse_WhenSuccessful()
    {
        await using var context = TestDbContextFactory.Create();
        var restaurantsToAdd = RestaurantResources.GetRestaurantsToAdd();

        await context.Restaurants.AddRangeAsync(restaurantsToAdd, TestContext.Current.CancellationToken);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var handler = new GetAllRestaurantsQueryHandler(context, _cache, _logger);

        var query = new GetAllRestaurantsQuery(Page: 1, PageSize: 3);

        var result = await handler.Handle(query, TestContext.Current.CancellationToken);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.HasNextPage.Should().BeTrue();
        result.Value.HasPreviousPage.Should().BeFalse();
        result.Value.TotalCount.Should().Be(6);
        result.Value.TotalPages.Should().Be(2);
        result.Value.Items.Count.Should().Be(3);
    }
}