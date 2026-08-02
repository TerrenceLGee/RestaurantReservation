using System.Net;
using System.Net.Http.Json;

using FluentAssertions;

using RestaurantReservation.Application.Features.Restaurants.Query.Response;

namespace RestaurantReservation.IntegrationTests.Restaurants.Tables;

public class AddTableTests(IntegrationTestWebAppFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task AddTable_Returns_RestaurantTableResponse_And_StatusCode_201Created_OnSuccess()
    {
        const string restaurantName = "Alex's Italian Steakhouse";
        const string groupName = "Outside Patio";
        var restaurantId = await CreateRestaurantForRetrieval(restaurantName);

        var tableResponse = await Client.PostAsJsonAsync($"/api/restaurants/{restaurantId}/tables/add",
            new { NumberOfSeats = 6, TableGroup = groupName },
            TestContext.Current.CancellationToken);

        var tableResult = await tableResponse.Content.ReadFromJsonAsync<RestaurantTableResponse>(
            TestContext.Current.CancellationToken);

        tableResponse.IsSuccessStatusCode.Should().BeTrue();
        tableResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        tableResult.Should().NotBeNull();
        tableResult.SeatsAtTable.Should().Be(6);
        tableResult.TableGroupName.Should().Be(groupName);
    }

    [Fact]
    public async Task AddTable_Returns_StatusCode_404NotFound_WhenTableGroup_IsNon_Existent()
    {
        const string restaurantName = "Alex's Steakhouse";
        const string groupName = "Balcony";

        var restaurantId = await CreateRestaurantForRetrieval(restaurantName);

        var tableResponse = await Client.PostAsJsonAsync($"/api/restaurants/{restaurantId}/tables/add",
            new { NumberOfSeats = 4, TableGroup = groupName },
            TestContext.Current.CancellationToken);

        tableResponse.IsSuccessStatusCode.Should().BeFalse();
        tableResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}