using System.Net;
using System.Net.Http.Json;

using FluentAssertions;

using RestaurantReservation.Application.Features.Reservations.Query.Responses;

namespace RestaurantReservation.IntegrationTests.Reservations;

public class RescheduleReservationTests(IntegrationTestWebAppFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task RescheduleReservation_Returns_RescheduledReservationDetailResponse_And_StatusCode_200OK_OnSuccess()
    {
        var reservationId = await ScheduleReservationForTesting();
        const string restaurantName = "Red Lobster";
        const string tableGroup = "Main Dining Room";

        var reservationResponse = await Client.PutAsJsonAsync("/api/reservations/reschedulereservation", new
            {
                ReservationId = reservationId,
                RestaurantName = restaurantName,
                RescheduleDate = new DateOnly(2026, 9, 2),
                RescheduleStartTime = new TimeOnly(15,00),
                RescheduleEndTime = new TimeOnly(17, 30),
                RescheduleNumberOfGuests = 6,
                RescheduleTableGroup = tableGroup
            },
            TestContext.Current.CancellationToken);

        var reservationResult =
            await reservationResponse.Content.ReadFromJsonAsync<RescheduledReservationDetailResponse>(
                TestContext.Current.CancellationToken);

        reservationResponse.IsSuccessStatusCode.Should().BeTrue();
        reservationResponse.Should().Be(HttpStatusCode.OK);
        reservationResult.Should().NotBeNull();
        reservationResult.OriginalNumberOfGuests.Should().Be(3);
        reservationResult.RescheduledNumberOfGuests.Should().Be(6);
        reservationResult.OriginalReservationDate.Should().Be(new DateOnly(2026, 9, 1));
        reservationResult.OriginalReservationStartTime.Should().Be(new TimeOnly(19, 00));
        reservationResult.OriginalReservationEndTime.Should().Be(new TimeOnly(20, 00));
        reservationResult.RescheduledReservationDate.Should().Be(new DateOnly(2026, 9, 2));
        reservationResult.RescheduledReservationStartTime.Should().Be(new TimeOnly(15,00));
        reservationResult.RescheduledReservationEndTime.Should().Be(new TimeOnly(17, 30));
    }
}