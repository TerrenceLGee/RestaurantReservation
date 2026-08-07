using System.Diagnostics;

using FluentAssertions;

using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

using NSubstitute;

using RestaurantReservation.Application.Features.Restaurants.Tables.Query.Get;
using RestaurantReservation.Application.Features.Restaurants.Tables.Query.Responses;
using RestaurantReservation.Domain.Common;
using RestaurantReservation.Domain.Restaurants;
using RestaurantReservation.Domain.Tables;
using RestaurantReservation.UnitTests.Resources;

namespace RestaurantReservation.UnitTests.Restaurants.Tables;

public class GetTableQueryHandlerTests
{
    private readonly HybridCache _cache = Substitute.For<HybridCache>();
    private readonly ILogger<GetTableQueryHandler> _logger = Substitute.For<ILogger<GetTableQueryHandler>>();

    public GetTableQueryHandlerTests()
    {
        _cache.GetOrCreateAsync(
                Arg.Any<string>(),
                Arg.Any<Func<CancellationToken, ValueTask<TableDetailResponse>>>(),
                Arg.Any<HybridCacheEntryOptions>(),
                Arg.Any<IEnumerable<string>>(),
                Arg.Any<CancellationToken>())
            .Returns<ValueTask<TableDetailResponse>>(async callInfo =>
            {
                var factory = callInfo.Arg<Func<CancellationToken, ValueTask<TableDetailResponse>>>();
                var token = callInfo.Arg<CancellationToken>();
                Debug.Assert(factory != null, nameof(factory) + " != null");
                return await factory(token);
            });
    }

    [Fact]
    public async Task GetTableQueryHandler_Returns_Result_Of_TableDetailResponse_OnSuccess()
    {
        await using var context = TestDbContextFactory.Create();

        (Restaurant restaurant, Table table) = RestaurantResources.GetTableToAdd(45, "Main Dining Room");

        await context.Restaurants.AddAsync(restaurant, TestContext.Current.CancellationToken);
        await context.Tables.AddAsync(table, TestContext.Current.CancellationToken);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var query = new GetTableQuery(restaurant.Id, table.Id);

        var handler = new GetTableQueryHandler(context, _cache, _logger);

        var result = await handler.Handle(query, TestContext.Current.CancellationToken);

        result.IsSuccess.Should().BeTrue();
        result.Error.ErrorType.Should().Be(ErrorType.None);
        result.Value.Should().NotBeNull();
        result.Value.IsInTableGroup.Should().BeTrue();
        result.Value.RestaurantId.Should().Be(restaurant.Id);
        result.Value.Id.Should().Be(table.Id);
        result.Value.RestaurantName.Should().Be("International House Of Culinary Delights");
        result.Value.TableGroupName.Should().Be("Main Dining Room");
    }
}