using System.Net;
using System.Net.Http.Json;

using FluentAssertions;

using RestaurantReservation.Application.Features.Reservations.Query.Responses;
using RestaurantReservation.Domain.Common;

namespace RestaurantReservation.IntegrationTests.Reservations;

public class GetAllRestaurantReservationsTests(IntegrationTestWebAppFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task
        GetAllRestaurantReservations_Page_3_PageSize_5_Returns_PagedResult_Of_ReservationResponse_And_StatusCode_200OK_OnSuccess()
    {
        const string restaurantName = "Mr. Brown's Barbecue Pit";

        var restaurantId = await CreateRestaurantForRetrieval(restaurantName);

        await ScheduleReservationsForTesting(restaurantName);

        await LoginAndSetAuthenticationAsync("admin@example.com", "Pa$$w0rd");

        var reservationsResponse = await Client.GetAsync($"/api/reservations/restaurants/{restaurantId}?page=3&pageSize=5",
            TestContext.Current.CancellationToken);

        var reservationsResult = await reservationsResponse.Content.ReadFromJsonAsync<PagedResult<ReservationResponse>>(
            TestContext.Current.CancellationToken);

        reservationsResponse.IsSuccessStatusCode.Should().BeTrue();
        reservationsResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        reservationsResult.Should().NotBeNull();
        reservationsResult.HasPreviousPage.Should().BeTrue();
        reservationsResult.HasNextPage.Should().BeTrue();
        reservationsResult.Items.Count.Should().Be(5);
        reservationsResult.TotalCount.Should().Be(31);
        reservationsResult.TotalPages.Should().Be(7);
        reservationsResult.Items[0].RestaurantName.Should().Be(restaurantName);
    }
}