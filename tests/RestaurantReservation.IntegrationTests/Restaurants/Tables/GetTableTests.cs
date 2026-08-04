using System.Net;
using System.Net.Http.Json;

using FluentAssertions;

using RestaurantReservation.Application.Features.Restaurants.Tables.Command.Add;
using RestaurantReservation.Application.Features.Restaurants.Tables.Query.Responses;

namespace RestaurantReservation.IntegrationTests.Restaurants.Tables;

public class GetTableTests(IntegrationTestWebAppFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task GetTable_Returns_TableDetailResponse_And_StatusCode_200OK_OnSuccess()
    {
        const string restaurantName = "Martha's Seafood By The Sea";
        const string groupName = "Outside Patio";

        var restaurantId = await CreateRestaurantForRetrieval(restaurantName);

        var tableId = await CreateTableForRetrieval(new AddTableCommand(
            restaurantId,
            4,
            groupName));

        var tableResponse = await Client.GetAsync($"/api/restaurants/{restaurantId}/tables/{tableId}",
            TestContext.Current.CancellationToken);

        var tableResult = await tableResponse.Content.ReadFromJsonAsync<TableDetailResponse>(
            TestContext.Current.CancellationToken);

        tableResponse.IsSuccessStatusCode.Should().BeTrue();
        tableResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        tableResult.Should().NotBeNull();
        tableResult.Id.Should().Be(tableId);
        tableResult.RestaurantId.Should().Be(restaurantId);
        tableResult.RestaurantName.Should().Be(restaurantName);
        tableResult.IsInTableGroup.Should().BeTrue();
        tableResult.TableGroupName.Should().Be(groupName);
        tableResult.SeatsAtTable.Should().Be(4);
    }

    [Fact]
    public async Task GetTable_Should_Return_StatusCode_404NotFound_When_TableId_Is_Invalid()
    { 
        const string restaurantName = "Smith's Famous Desert House";

        var restaurantId = await CreateRestaurantForRetrieval(restaurantName);

        var tableId = Guid.CreateVersion7();
        
        var tableResponse = await Client.GetAsync($"/api/restaurants/{restaurantId}/tables/{tableId}",
            TestContext.Current.CancellationToken);

        tableResponse.IsSuccessStatusCode.Should().BeFalse();
        tableResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}