using FluentAssertions;

using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

using NSubstitute;

using RestaurantReservation.Application.Abstractions;
using RestaurantReservation.Application.Features.Reservations.Command.Schedule;
using RestaurantReservation.Domain.Common;
using RestaurantReservation.UnitTests.Resources;

namespace RestaurantReservation.UnitTests.Reservations;

public class ScheduleReservationCommandHandlerTests
{
    private readonly HybridCache _cache = Substitute.For<HybridCache>();

    private readonly ILogger<ScheduleReservationCommandHandler> _logger =
        Substitute.For<ILogger<ScheduleReservationCommandHandler>>();

    private readonly ICurrentUser _currentUser = Substitute.For<ICurrentUser>();
    private readonly Guid _customerId = Guid.CreateVersion7();

    [Fact]
    public async Task ScheduleReservationCommandHandler_Returns_Result_Of_ReservationDetailResponse_OnSuccess()
    {
        await using var context = TestDbContextFactory.Create();

        var addRestaurantCommand = RestaurantResources.AddRestaurant();
        var restaurant = RestaurantResources.GetRestaurantToAdd(addRestaurantCommand);

        await context.Restaurants.AddAsync(restaurant, TestContext.Current.CancellationToken);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        const string groupName = "VIP Section";

        _currentUser.UserId.Returns(_customerId.ToString());

        var command = RestaurantResources.AddReservation(restaurant.Name, groupName);

        var handler = new ScheduleReservationCommandHandler(context, _cache, _currentUser, _logger);

        var result = await handler.Handle(command, TestContext.Current.CancellationToken);

        result.IsSuccess.Should().BeTrue();
        result.Error.ErrorType.Should().Be(ErrorType.None);
        result.Value.RestaurantId.Should().Be(restaurant.Id);
        result.Value.RestaurantName.Should().Be(restaurant.Name);
        result.Value.NumberOfGuests.Should().Be(3);
        result.Value.CustomerEmail.Should().Be("customer@example.com");
        result.Value.ReservationDate.Should().Be(new DateOnly(2026, 10, 1));
        result.Value.ReservationStartTime.Should().Be(new TimeOnly(15,30));
        result.Value.ReservationEndTime.Should().Be(new TimeOnly(17, 30));
    }

    [Fact]
    public async Task
        ScheduleReservationCommandHandler_Returns_FailureResult_When_Scheduling_Reservation_At_InvalidTime()
    {
        await using var context = TestDbContextFactory.Create();

        var addRestaurantCommand = RestaurantResources.AddRestaurant();
        var restaurant = RestaurantResources.GetRestaurantToAdd(addRestaurantCommand);

        await context.Restaurants.AddAsync(restaurant, TestContext.Current.CancellationToken);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        const string groupName = "VIP Section";

        _currentUser.UserId.Returns(_customerId.ToString());
        
        var command = new ScheduleReservationCommand(
            restaurant.Name,
            "Dennis",
            "Edwards",
            "customer@example.com",
            "555-123-4567",
            new DateOnly(2026, 10, 1),
            new TimeOnly(21,30),
            new TimeOnly(23, 30),
            3,
            groupName);

        var handler = new ScheduleReservationCommandHandler(context, _cache, _currentUser, _logger);

        var result = await handler.Handle(command, TestContext.Current.CancellationToken);

        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("Restaurant.HoursOutOfRange");
        result.Error.ErrorType.Should().Be(ErrorType.BadRequest);
    }
}