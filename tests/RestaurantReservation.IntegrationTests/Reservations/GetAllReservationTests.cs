using System.Net;
using System.Net.Http.Json;

using FluentAssertions;

using RestaurantReservation.Application.Features.Reservations.Query.Responses;
using RestaurantReservation.Domain.Common;

namespace RestaurantReservation.IntegrationTests.Reservations;

public class GetAllReservationTests(IntegrationTestWebAppFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task GetAllReservations_Page_2_PageSize_10_Returns_PagedResult_Of_ReservationResponse_And_StatusCode_200OK_OnSuccess()
    {
        const string restaurantName = "Red Lobster";
        
        await ScheduleReservationsForTesting(restaurantName);
        
        const string customerEmail = "customer@example.com";

        var reservationsResponse = await Client.GetAsync("/api/reservations?page=2&pageSize=10",
            TestContext.Current.CancellationToken);

        var reservationsResult = await reservationsResponse.Content.ReadFromJsonAsync<PagedResult<ReservationResponse>>(
            TestContext.Current.CancellationToken);

        reservationsResponse.IsSuccessStatusCode.Should().BeTrue();
        reservationsResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        reservationsResult.Should().NotBeNull();
        reservationsResult.HasPreviousPage.Should().BeTrue();
        reservationsResult.HasNextPage.Should().BeTrue();
        reservationsResult.Items.Count.Should().Be(10);
        reservationsResult.TotalCount.Should().Be(31);
        reservationsResult.TotalPages.Should().Be(4);
        reservationsResult.Items[0].RestaurantName.Should().Be(restaurantName);
        reservationsResult.Items[0].CustomerEmail.Should().Be(customerEmail);
    }
}