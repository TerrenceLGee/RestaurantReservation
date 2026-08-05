using System.Net;
using System.Net.Http.Json;

using FluentAssertions;

using RestaurantReservation.Application.Features.Reservations.Query.Responses;

namespace RestaurantReservation.IntegrationTests.Reservations;

public class ScheduleReservationTests(IntegrationTestWebAppFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task ScheduleReservation_Returns_ReservationDetailResponse_And_StatusCode_201Created_On_Success()
    {
        await LoginAndSetAuthenticationAsync("customer@example.com", "Pa$$w0rd");
        
        const string restaurantName = "Red Lobster";
        const string tableGroup = "Main Dining Room";

        var reservationResponse = await Client.PostAsJsonAsync("/api/reservations/schedulereservation",
            new
            {
                RestaurantName = restaurantName,
                CustomerFirstName = "Dennis",
                CustomerLastName = "Edwards",
                CustomerEmail = "customer@example.com",
                CustomerPhone = "555-123-4567",
                ReservationDate = new DateOnly(2026, 08, 29),
                ReservationStartTime = new TimeOnly(15, 30),
                ReservationEndTime = new TimeOnly(18, 30),
                NumberOfGuests = 10,
                TableGroup = tableGroup
            },
            TestContext.Current.CancellationToken);

        var reservationResult = await reservationResponse.Content.ReadFromJsonAsync<ReservationDetailResponse>(
            TestContext.Current.CancellationToken);

        reservationResponse.IsSuccessStatusCode.Should().BeTrue();
        reservationResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        reservationResult.Should().NotBeNull();
        reservationResult.NumberOfGuests.Should().Be(10);
        reservationResult.ReservationDate.Should().Be(new DateOnly(2026, 08, 29));
        reservationResult.ReservationStartTime.Should().Be(new TimeOnly(15, 30));
        reservationResult.ReservationEndTime.Should().Be(new TimeOnly(18, 30));
        reservationResult.CustomerEmail.Should().Be("customer@example.com");
        reservationResult.RestaurantName.Should().Be(restaurantName);
    }

    [Fact]
    public async Task
        ScheduleReservation_Returns_StatusCode_404NotFound_When_SchedulingReservation_At_Invalid_Restaurant()
    {
        await LoginAndSetAuthenticationAsync("customer@example.com", "Pa$$w0rd");

        const string restaurantName = "Mario and Luigi's Italian Bar And Grill";
        const string tableGroup = "VIP Section";
        
        var reservationResponse = await Client.PostAsJsonAsync("/api/reservations/schedulereservation",
            new
            {
                RestaurantName = restaurantName,
                CustomerFirstName = "Dennis",
                CustomerLastName = "Edwards",
                CustomerEmail = "customer@example.com",
                CustomerPhone = "123-456-7890",
                ReservationDate = new DateOnly(2026, 08, 29),
                ReservationStartTime = new TimeOnly(15, 30),
                ReservationEndTime = new TimeOnly(18, 30),
                NumberOfGuests = 10,
                TableGroup = tableGroup
            },
            TestContext.Current.CancellationToken);

        reservationResponse.IsSuccessStatusCode.Should().BeFalse();
        reservationResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}