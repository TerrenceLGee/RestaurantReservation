using System.Net;
using System.Net.Http.Json;

using FluentAssertions;

using RestaurantReservation.Application.Features.Restaurants.Query.Response;

namespace RestaurantReservation.IntegrationTests.Restaurants;

public class GetRestaurantTests(IntegrationTestWebAppFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task GetRestaurant_ShouldReturnRestaurant_WhenRestaurant_Is_Valid()
    {
        const string restaurantName = "International House Of Culinary Delights";
        var restaurantId = await CreateRestaurantForRetrieval(restaurantName);

        var restaurantResponse = await Client.GetAsync($"/api/restaurants/{restaurantId}",
            TestContext.Current.CancellationToken);

        var restaurantResult = await restaurantResponse.Content.ReadFromJsonAsync<RestaurantDetailResponse>(
            TestContext.Current.CancellationToken);

        restaurantResponse.IsSuccessStatusCode.Should().BeTrue();
        restaurantResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        restaurantResult.Should().NotBeNull();
        restaurantResult.Id.Should().Be(restaurantId);
        restaurantResult.Name.Should().Be(restaurantName);
        restaurantResult.Schedule[0].DailyHours[0].Should().Be(new TimeOnly(10, 00));
    }

    [Fact]
    public async Task GetRestaurant_ShouldFail_WhenRestaurantId_Is_Not_Valid()
    {
        var restaurantId = Guid.CreateVersion7();

        var restaurantResponse = await Client.GetAsync($"/api/restaurants/{restaurantId}",
            TestContext.Current.CancellationToken);

        var restaurantResult = await restaurantResponse.Content.ReadFromJsonAsync<RestaurantDetailResponse>(
            TestContext.Current.CancellationToken);

        restaurantResponse.IsSuccessStatusCode.Should().BeFalse();
        restaurantResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        restaurantResult.Should().NotBeNull();
        restaurantResult.Schedule.Should().BeNull();
        restaurantResult.TableInfo.Should().BeNull();
    }
}