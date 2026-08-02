using System.Net;
using System.Net.Http.Json;

using FluentAssertions;

using RestaurantReservation.Application.Features.Restaurants.Query.Response;
using RestaurantReservation.Domain.Common;

namespace RestaurantReservation.IntegrationTests.Restaurants;

public class GetAllRestaurantsTests(IntegrationTestWebAppFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task GetAllRestaurants_ShouldReturnRestaurants_WhenSuccessful()
    {
        var restaurantResponse = await Client.GetAsync("/api/restaurants",
            TestContext.Current.CancellationToken);

        var restaurantResult = await
            restaurantResponse.Content.ReadFromJsonAsync<PagedResult<RestaurantResponse>>(
                TestContext.Current.CancellationToken);

        restaurantResponse.IsSuccessStatusCode.Should().BeTrue();
        restaurantResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        restaurantResult.Should().NotBeNull();
        restaurantResult.TotalPages.Should().Be(1);
        restaurantResult.TotalCount.Should().Be(6);
        restaurantResult.HasNextPage.Should().BeFalse();
        restaurantResult.HasPreviousPage.Should().BeFalse();
    }

    [Fact]
    public async Task GetAllRestaurants_WithPage_1_PageSize_3_ShouldReturnRestaurants_WhenSuccessful()
    {
        var restaurantResponse = await Client.GetAsync("/api/restaurants?page=1&pageSize=3",
            TestContext.Current.CancellationToken);

        var restaurantResult =
            await restaurantResponse.Content.ReadFromJsonAsync<PagedResult<RestaurantResponse>>(
                TestContext.Current.CancellationToken);

        restaurantResponse.IsSuccessStatusCode.Should().BeTrue();
        restaurantResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        restaurantResult.Should().NotBeNull();
        restaurantResult.TotalPages.Should().Be(2);
        restaurantResult.TotalCount.Should().Be(6);
        restaurantResult.HasNextPage.Should().BeTrue();
        restaurantResult.HasPreviousPage.Should().BeFalse();
        restaurantResult.Items.Count.Should().Be(3);
    }

    [Fact]
    public async Task GetAllRestaurants_WithPageSize_10_And_Page_2_ShouldReturnEmpty_RestaurantsCollection()
    {
        var restaurantResponse = await Client.GetAsync("/api/restaurants?page=2&pageSize=10",
            TestContext.Current.CancellationToken);

        var restaurantResult =
            await restaurantResponse.Content.ReadFromJsonAsync<PagedResult<RestaurantResponse>>(
                TestContext.Current.CancellationToken);

        restaurantResponse.IsSuccessStatusCode.Should().BeTrue();
        restaurantResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        restaurantResult.Should().NotBeNull();
        restaurantResult.HasPreviousPage.Should().BeTrue();
        restaurantResult.HasNextPage.Should().BeFalse();
        restaurantResult.TotalCount.Should().Be(6);
        restaurantResult.Items.Count.Should().Be(0);
        restaurantResult.TotalPages.Should().Be(1);
    }
}