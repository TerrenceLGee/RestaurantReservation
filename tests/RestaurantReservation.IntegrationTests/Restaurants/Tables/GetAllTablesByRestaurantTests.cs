using System.Net;
using System.Net.Http.Json;

using FluentAssertions;

using RestaurantReservation.Application.Features.Restaurants.Tables.Query.Responses;
using RestaurantReservation.Domain.Common;

namespace RestaurantReservation.IntegrationTests.Restaurants.Tables;

public class GetAllTablesByRestaurantTests(IntegrationTestWebAppFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task GetAllTablesByRestaurant_Page2_PageSize15_Returns_PagedResult_Of_TableResponse_And_StatusCode_200OK_OnSuccess()
    {
        const string restaurantName = "Reginald's Crab Shack";

        var restaurantId = await CreateRestaurantForRetrieval(restaurantName);

        var tablesResponse = await Client.GetAsync($"/api/restaurants/{restaurantId}/tables?page=2&pageSize=15",
            TestContext.Current.CancellationToken);

        var tablesResult = await tablesResponse.Content.ReadFromJsonAsync<PagedResult<TableResponse>>(
            TestContext.Current.CancellationToken);

        tablesResponse.IsSuccessStatusCode.Should().BeTrue();
        tablesResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        tablesResult.Should().NotBeNull();
        tablesResult.Items.Count.Should().Be(15);
        tablesResult.TotalCount.Should().Be(105);
        tablesResult.TotalPages.Should().Be(7);
        tablesResult.HasNextPage.Should().BeTrue();
        tablesResult.HasPreviousPage.Should().BeTrue();
    }

    [Fact]
    public async Task GetAllTablesByRestaurant_Returns_StatusCode_404NotFound_When_RestaurantId_IsInvalid()
    {
        await LoginAndSetAuthenticationAsync("admin@example.com", "Pa$$w0rd");
        
        var restaurantId = Guid.CreateVersion7();
        
        var tablesResponse = await Client.GetAsync($"/api/restaurants/{restaurantId}/tables",
            TestContext.Current.CancellationToken);

        tablesResponse.IsSuccessStatusCode.Should().BeFalse();
        tablesResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}