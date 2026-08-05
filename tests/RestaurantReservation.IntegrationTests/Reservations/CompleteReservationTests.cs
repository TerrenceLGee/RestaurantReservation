using System.Net;
using System.Net.Http.Json;

using FluentAssertions;

using RestaurantReservation.Application.Features.Reservations.Query.Responses;

namespace RestaurantReservation.IntegrationTests.Reservations;

public class CompleteReservationTests(IntegrationTestWebAppFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task CompleteReservation_Returns_ReservationDetailResult_And_StatusCode_200OK_OnSuccess()
    {
        var reservationId = await ScheduleReservationForTesting();
        const string restaurantName = "Red Lobster";

        var reservationResponse = await Client.PutAsJsonAsync("/api/reservations/completereservation",
            new
            {
                ReservationId = reservationId,
                RestaurantName = restaurantName,
                ReservationDate = new DateOnly(2026, 9, 1),
                ReservationStartTime = new TimeOnly(19, 00),
                ReservationEndTime = new TimeOnly(20, 00)
            },
            TestContext.Current.CancellationToken);

        var reservationResult = await reservationResponse.Content.ReadFromJsonAsync<ReservationDetailResponse>(
            TestContext.Current.CancellationToken);

        reservationResponse.IsSuccessStatusCode.Should().BeTrue();
        reservationResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        reservationResult.Should().NotBeNull();
        reservationResult.ReservationCompletedAtUtc.Should().NotBeNull();
        reservationResult.RestaurantName.Should().Be(restaurantName);
    }

    [Fact]
    public async Task CompleteReservation_Returns_StatusCode_404NotFound_When_ReservationId_Is_Invalid()
    {
        await LoginAndSetAuthenticationAsync("customer@example.com", "Pa$$w0rd");
        var reservationId = Guid.CreateVersion7();
        const string restaurantName = "Red Lobster";
        
        var reservationResponse = await Client.PutAsJsonAsync("/api/reservations/completereservation",
            new
            {
                ReservationId = reservationId,
                RestaurantName = restaurantName,
                ReservationDate = new DateOnly(2026, 9, 1),
                ReservationStartTime = new TimeOnly(19, 00),
                ReservationEndTime = new TimeOnly(20, 00)
            },
            TestContext.Current.CancellationToken);

        reservationResponse.IsSuccessStatusCode.Should().BeFalse();
        reservationResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}