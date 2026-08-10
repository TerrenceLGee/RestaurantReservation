using System.Diagnostics;

using FluentAssertions;

using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

using NSubstitute;

using RestaurantReservation.Application.Features.Restaurants.TableGroups.Query.GetAll;
using RestaurantReservation.Application.Features.Restaurants.TableGroups.Query.Responses;
using RestaurantReservation.Domain.Common;
using RestaurantReservation.UnitTests.Resources;

namespace RestaurantReservation.UnitTests.Restaurants.TableGroups;

public class GetAllTableGroupsQueryHandlerTests
{
    private readonly HybridCache _cache = Substitute.For<HybridCache>();

    private readonly ILogger<GetAllTableGroupsQueryHandler> _logger =
        Substitute.For<ILogger<GetAllTableGroupsQueryHandler>>();

    public GetAllTableGroupsQueryHandlerTests()
    {
        _cache.GetOrCreateAsync(
                Arg.Any<string>(),
                Arg.Any<Func<CancellationToken, ValueTask<(List<TableGroupResponse> items, int totalCount)>>>(),
                Arg.Any<HybridCacheEntryOptions>(),
                Arg.Any<IEnumerable<string>>(),
                Arg.Any<CancellationToken>())
            .Returns<ValueTask<(List<TableGroupResponse> items, int totalCount)>>(async callInfo =>
            {
                var factory = callInfo
                    .Arg<Func<CancellationToken, ValueTask<(List<TableGroupResponse> items, int totalCount)>>>();
                var token = callInfo.Arg<CancellationToken>();
                Debug.Assert(factory != null, nameof(factory) + " != null");
                return await factory(token);
            });
    }

    [Fact]
    public async Task
        GetAllTableGroupsQueryHandler_Returns_Result_Of_PagedResult_Page_2_PageSize_2_TableGroupResponse_OnSuccess()
    {
        await using var context = TestDbContextFactory.Create();

        var addRestaurantCommand = RestaurantResources.AddRestaurant();
        var restaurant = RestaurantResources.GetRestaurantToAdd(addRestaurantCommand);

        await context.Restaurants.AddAsync(restaurant, TestContext.Current.CancellationToken);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var command = new GetAllTableGroupsQuery(
            restaurant.Id,
            2,
            2);

        var handler = new GetAllTableGroupsQueryHandler(context, _cache, _logger);
        var result = await handler.Handle(command, TestContext.Current.CancellationToken);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Error.ErrorType.Should().Be(ErrorType.None);
        result.Value.HasNextPage.Should().BeFalse();
        result.Value.HasPreviousPage.Should().BeTrue();
        result.Value.TotalCount.Should().Be(4);
        result.Value.Items.Count.Should().Be(2);
        result.Value.TotalPages.Should().Be(2);
    }

    [Fact]
    public async Task GetAllTableGroupsQueryHandler_Returns_FailureResult_When_Restaurant_IsNonExistent()
    {
        await using var context = TestDbContextFactory.Create();

        var restaurantId = Guid.CreateVersion7();

        var command = new GetAllTableGroupsQuery(restaurantId);

        var handler = new GetAllTableGroupsQueryHandler(context, _cache, _logger);

        var result = await handler.Handle(command, TestContext.Current.CancellationToken);

        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("TableGroups.NotFound");
        result.Error.ErrorType.Should().Be(ErrorType.NotFound);
    }
}