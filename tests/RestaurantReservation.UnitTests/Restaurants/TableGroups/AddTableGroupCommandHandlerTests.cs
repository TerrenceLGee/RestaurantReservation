using FluentAssertions;

using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

using NSubstitute;

using RestaurantReservation.Application.Features.Restaurants.TableGroups.Command.Add;
using RestaurantReservation.Domain.Common;
using RestaurantReservation.UnitTests.Resources;

namespace RestaurantReservation.UnitTests.Restaurants.TableGroups;

public class AddTableGroupCommandHandlerTests
{
    private readonly HybridCache _cache = Substitute.For<HybridCache>();

    private readonly ILogger<AddTableGroupCommandHandler> _logger =
        Substitute.For<ILogger<AddTableGroupCommandHandler>>();

    [Fact]
    public async Task AddTableGroupCommandHandler_Should_Return_Result_Of_TableGroupDetailResponse_OnSuccess()
    {
        await using var context = TestDbContextFactory.Create();

        var addRestaurantCommand = RestaurantResources.AddRestaurant();
        var restaurant = RestaurantResources.GetRestaurantToAdd(addRestaurantCommand);

        await context.Restaurants.AddAsync(restaurant, TestContext.Current.CancellationToken);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        const string restaurantName = "International House Of Culinary Delights";
        const string groupName = "C# Developers";

        var command = new AddTableGroupCommand(
            restaurant.Id,
            groupName,
            15,
            4);

        var handler = new AddTableGroupCommandHandler(context, _cache, _logger);
        var result = await handler.Handle(command, TestContext.Current.CancellationToken);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.GroupName.Should().Be(groupName);
        result.Value.NumberOfTables.Should().Be(15);
        result.Value.RestaurantId.Should().Be(restaurant.Id);
        result.Value.RestaurantName.Should().Be(restaurantName);
    }

    [Fact]
    public async Task
        AddTableGroupCommandHandler_Should_Return_FailureResult_When_Trying_To_Add_New_TableGroup_With_Name_Of_Already_Created_TableGroup()
    {
        await using var context = TestDbContextFactory.Create();

        var addRestaurantCommand = RestaurantResources.AddRestaurant();
        var restaurant = RestaurantResources.GetRestaurantToAdd(addRestaurantCommand);

        await context.Restaurants.AddAsync(restaurant, TestContext.Current.CancellationToken);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        const string groupName = "Outside Patio";

        var command = new AddTableGroupCommand(restaurant.Id, groupName, 45, 3);
        var handler = new AddTableGroupCommandHandler(context, _cache, _logger);

        var result = await handler.Handle(command, TestContext.Current.CancellationToken);

        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("TableGroup.AlreadyExists");
        result.Error.ErrorType.Should().Be(ErrorType.Conflict);
    }
}