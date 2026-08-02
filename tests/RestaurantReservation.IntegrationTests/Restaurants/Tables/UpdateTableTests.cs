using System.Net;
using System.Net.Http.Json;

using FluentAssertions;

using RestaurantReservation.Application.Features.Restaurants.Tables.Command.Add;
using RestaurantReservation.Application.Features.Restaurants.Tables.Query.Responses;

namespace RestaurantReservation.IntegrationTests.Restaurants.Tables;

public class UpdateTableTests(IntegrationTestWebAppFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task UpdateTable_Returns_StatusCode_204NoContent_OnSuccess()
    {
        const string restaurantName = "Gia's Desert Shop";
        const string groupName = "Main Dining Room";
        const string updatedGroupName = "Family Dining Room";

        var restaurantId = await CreateRestaurantForRetrieval(restaurantName);

        var tableId = await CreateTableForRetrieval(new AddTableCommand(
            restaurantId,
            4,
            groupName));

        var tableResponse = await Client.PutAsJsonAsync($"/api/restaurants/{restaurantId}/tables/update/{tableId}",
            new { NumberOfSeats = 6, GroupName = updatedGroupName },
            TestContext.Current.CancellationToken);

        var updatedTable = await GetTableToCheckProperties(restaurantId, tableId);

        tableResponse.IsSuccessStatusCode.Should().BeTrue();
        tableResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
        updatedTable.SeatsAtTable.Should().Be(6);
        updatedTable.TableGroupName.Should().Be(updatedGroupName);
    }

    [Fact]
    public async Task UpdateTableGroup_Returns_StatusCode_404NotFound_When_TableId_IsInvalid()
    {
        const string restaurantName = "Foxy's Bar And Grill";
        const string updatedGroupName = "Family Dining Room";

        var restaurantId = await CreateRestaurantForRetrieval(restaurantName);

        var tableId = Guid.CreateVersion7();
        
        var tableResponse = await Client.PutAsJsonAsync($"/api/restaurants/{restaurantId}/tables/update/{tableId}",
            new { NumberOfSeats = 6, GroupName = updatedGroupName },
            TestContext.Current.CancellationToken);

        tableResponse.IsSuccessStatusCode.Should().BeFalse();
        tableResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private async Task<TableDetailResponse> GetTableToCheckProperties(Guid restaurantId, Guid tableId)
    {
        var tableResponse = await Client.GetAsync($"/api/restaurants/{restaurantId}/tables/{tableId}",
            TestContext.Current.CancellationToken);

        tableResponse.EnsureSuccessStatusCode();

        var tableResult = await tableResponse.Content.ReadFromJsonAsync<TableDetailResponse>(
            TestContext.Current.CancellationToken);

        tableResult.Should().NotBeNull();

        return tableResult;
    }
}