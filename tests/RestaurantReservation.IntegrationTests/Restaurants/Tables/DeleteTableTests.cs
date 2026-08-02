using System.Net;

using FluentAssertions;

using RestaurantReservation.Application.Features.Restaurants.Tables.Command.Add;

namespace RestaurantReservation.IntegrationTests.Restaurants.Tables;

public class DeleteTableTests(IntegrationTestWebAppFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task DeleteTable_Returns_StatusCode_204NoContent_OnSuccess()
    {
        const string restaurantName = "Bruce's All You Can Eat Burger Buffet";
        const string groupName = "Main Dining Room";

        var restaurantId = await CreateRestaurantForRetrieval(restaurantName);

        var tableId = await CreateTableForRetrieval(new AddTableCommand(
            restaurantId,
            4,
            groupName));

        var tableResponse = await Client.DeleteAsync($"/api/restaurants/{restaurantId}/tables/delete/{tableId}",
            TestContext.Current.CancellationToken);

        tableResponse.IsSuccessStatusCode.Should().BeTrue();
        tableResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteTable_Returns_StatusCode_404NotFound_When_TableId_IsInvalid()
    {
        const string restaurantName = "Bobby's Barbecue Pit";

        var restaurantId = await CreateRestaurantForRetrieval(restaurantName);

        var tableId = Guid.CreateVersion7();
        
        var tableResponse = await Client.DeleteAsync($"/api/restaurants/{restaurantId}/tables/delete/{tableId}",
            TestContext.Current.CancellationToken);

        tableResponse.IsSuccessStatusCode.Should().BeFalse();
        tableResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}