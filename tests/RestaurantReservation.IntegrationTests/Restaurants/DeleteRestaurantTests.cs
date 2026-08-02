using System.Net;

using FluentAssertions;

namespace RestaurantReservation.IntegrationTests.Restaurants;

public class DeleteRestaurantTests(IntegrationTestWebAppFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task DeleteRestaurant_Should_Return_204NoContent_WhenSuccessful()
    {
        var restaurantId = await CreateRestaurantForRetrieval("George's Steakhouse");

        var restaurantResponse = await Client.DeleteAsync($"/api/restaurants/delete/{restaurantId}",
            TestContext.Current.CancellationToken);

        restaurantResponse.IsSuccessStatusCode.Should().BeTrue();
        restaurantResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteRestaurant_Should_Return_404NotFound_WhenRestaurantDoesNotExist()
    {
        await LoginAndSetAuthenticationAsync("admin@example.com", "Pa$$w0rd");

        var restaurantId = Guid.CreateVersion7();

        var restaurantResponse = await Client.DeleteAsync($"/api/restaurants/delete/{restaurantId}",
            TestContext.Current.CancellationToken);

        restaurantResponse.IsSuccessStatusCode.Should().BeFalse();
        restaurantResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}