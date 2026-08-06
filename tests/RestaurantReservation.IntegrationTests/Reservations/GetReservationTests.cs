using System.Net;
using System.Net.Http.Json;

using FluentAssertions;

using RestaurantReservation.Application.Features.Reservations.Query.Responses;

namespace RestaurantReservation.IntegrationTests.Reservations;

public class GetReservationTests(IntegrationTestWebAppFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task GetReservation_Returns_ReservationDetailResponse_And_StatusCode_200OK_OnSuccess()
    {
        var reservationId = await ScheduleReservationForTesting();

        var reservationResponse = await Client.GetAsync($"/api/reservations/{reservationId}",
            TestContext.Current.CancellationToken);

        var reservationResult = await reservationResponse.Content.ReadFromJsonAsync<ReservationDetailResponse>(
            TestContext.Current.CancellationToken);

        reservationResponse.IsSuccessStatusCode.Should().BeTrue();
        reservationResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        reservationResult.Should().NotBeNull();
        reservationResult.NumberOfGuests.Should().Be(3);
        reservationResult.ReservationCompletedAtUtc.Should().BeNull();
        reservationResult.ReservationDate.Should().Be(new DateOnly(2026, 9, 1));
        reservationResult.ReservationStartTime.Should().Be(new TimeOnly(19, 00));
        reservationResult.ReservationEndTime.Should().Be(new TimeOnly(20, 00));
    }

    [Fact]
    public async Task GetReservation_Returns_StatusCode_404NotFound_When_ReservationId_Is_Invalid()
    {
        await LoginAndSetAuthenticationAsync("customer@example.com", "Pa$$w0rd");
        
        var reservationId = Guid.CreateVersion7();
        
        var reservationResponse = await Client.GetAsync($"/api/reservations/{reservationId}",
            TestContext.Current.CancellationToken);

        reservationResponse.IsSuccessStatusCode.Should().BeFalse();
        reservationResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}