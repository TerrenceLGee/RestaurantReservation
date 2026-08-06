using FluentAssertions;

using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

using NSubstitute;

using RestaurantReservation.Application.Features.Restaurants.Command.Add;
using RestaurantReservation.Domain.Common;
using RestaurantReservation.UnitTests.Resources;

namespace RestaurantReservation.UnitTests.Restaurants;

public class AddRestaurantCommandHandlerTests
{
    private readonly HybridCache _cache = Substitute.For<HybridCache>();

    private readonly ILogger<AddRestaurantCommandHandler> _logger =
        Substitute.For<ILogger<AddRestaurantCommandHandler>>();

    [Fact]
    public async Task AddRestaurantCommandHandler_Should_Return_Result_Of_RestaurantAddedResponse_OnSuccess()
    {
        await using var context = TestDbContextFactory.Create();
        var handler = new AddRestaurantCommandHandler(context, _cache, _logger);

        var command = RestaurantResources.AddRestaurant();

        var result = await handler.Handle(command, TestContext.Current.CancellationToken);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Name.Should().Be("International House Of Culinary Delights");
        result.Value.TotalTables.Should().Be(132);
        result.Value.Schedule[6].DailyHours[0].Should().Be(new TimeOnly(9, 0));
        result.Value.TableGroups.Count.Should().Be(4);
        result.Error.ErrorType.Should().Be(ErrorType.None);
    }

    [Fact]
    public async Task AddRestaurantCommandHandler_Should_Return_FailureResult_When_Restaurant_Already_Exists()
    {
        await using var context = TestDbContextFactory.Create();
        var handler = new AddRestaurantCommandHandler(context, _cache, _logger);
        var command = RestaurantResources.AddRestaurant();

        _ = await handler.Handle(command, TestContext.Current.CancellationToken);

        var result = await handler.Handle(command, TestContext.Current.CancellationToken);

        result.IsFailure.Should().BeTrue();
        result.Error.ErrorType.Should().Be(ErrorType.Conflict);
        result.Error.Code.Should().Be("Restaurant.AlreadyExistsInSystem");
    }
}