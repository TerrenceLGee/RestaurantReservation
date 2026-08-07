using System.Diagnostics;

using FluentAssertions;

using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

using NSubstitute;

using RestaurantReservation.Application.Features.Restaurants.Query.Get;
using RestaurantReservation.Application.Features.Restaurants.Query.Response;
using RestaurantReservation.Domain.Common;
using RestaurantReservation.UnitTests.Resources;

namespace RestaurantReservation.UnitTests.Restaurants;

public class GetRestaurantQueryHandlerTests
{
    private readonly HybridCache _cache;
    private readonly ILogger<GetRestaurantQueryHandler> _logger;

    public GetRestaurantQueryHandlerTests()
    {
        _cache = Substitute.For<HybridCache>();
        _logger = Substitute.For<ILogger<GetRestaurantQueryHandler>>();
        _cache.GetOrCreateAsync(
                Arg.Any<string>(),
                Arg.Any<Func<CancellationToken, ValueTask<RestaurantDetailResponse>>>(),
                Arg.Any<HybridCacheEntryOptions>(),
                Arg.Any<IEnumerable<string>>(),
                Arg.Any<CancellationToken>())
            .Returns<ValueTask<RestaurantDetailResponse>>(async callInfo =>
            {
                var factory = callInfo.Arg<Func<CancellationToken, ValueTask<RestaurantDetailResponse>>>();
                var token = callInfo.Arg<CancellationToken>();
                Debug.Assert(factory != null, nameof(factory) + " != null");
                return await factory(token);
            });
    }

    [Fact]
    public async Task GetRestaurant_Should_Return_Result_Of_RestaurantDetailResponse_OnSuccess()
    {
        await using var context = TestDbContextFactory.Create();

        var addRestaurantCommand = RestaurantResources.AddRestaurant();

        var restaurant = RestaurantResources.GetRestaurantToAdd(addRestaurantCommand);

        await context.Restaurants.AddAsync(restaurant, TestContext.Current.CancellationToken);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var handler = new GetRestaurantQueryHandler(context, _cache, _logger);

        var query = new GetRestaurantQuery(restaurant.Id);

        var result = await handler.Handle(query, TestContext.Current.CancellationToken);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Name.Should().Be("International House Of Culinary Delights");
        result.Value.TableGroups.Count.Should().Be(4);
        result.Value.TableInfo.TotalCount.Should().Be(132);
    }

    [Fact]
    public async Task GetRestaurant_Should_Return_FailureResult_When_Attempting_To_Retrieve_Invalid_Restaurant()
    {
        var restaurantId = Guid.CreateVersion7();

        await using var context = TestDbContextFactory.Create();

        var handler = new GetRestaurantQueryHandler(context, _cache, _logger);

        var query = new GetRestaurantQuery(restaurantId);

        var result = await handler.Handle(query, TestContext.Current.CancellationToken);

        result.IsFailure.Should().BeTrue();
        result.Error.ErrorType.Should().Be(ErrorType.NotFound);
        result.Error.Code.Should().Be("Restaurant.NotFound");
    }
}