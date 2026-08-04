using System.Net;
using System.Net.Http.Json;

using FluentAssertions;

using RestaurantReservation.Application.Features.Restaurants.Tables.Query.Responses;
using RestaurantReservation.Domain.Common;

namespace RestaurantReservation.IntegrationTests.Restaurants.Tables;

public class GetAllTablesByTableGroupTests(IntegrationTestWebAppFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task GetAllTablesByTableGroup_Page1_PageSize_15_Returns_PagedResult_Of_TableGroupResponse_And_StatusCode_200OK_OnSuccess()
    {
        const string restaurantName = "Terry's Teriyaki Palace";
        const string tableGroupName = "Main Dining Room";

        var restaurantId = await CreateRestaurantForRetrieval(restaurantName);

        var tablesResponse = await Client.GetAsync($"/api/restaurants/{restaurantId}/tables/{tableGroupName}?page=1&pageSize=15",
            TestContext.Current.CancellationToken);

        var tablesResult = await tablesResponse.Content.ReadFromJsonAsync<PagedResult<TableResponse>>(
            TestContext.Current.CancellationToken);

        tablesResponse.IsSuccessStatusCode.Should().BeTrue();
        tablesResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        tablesResult.Should().NotBeNull();
        tablesResult.HasPreviousPage.Should().BeFalse();
        tablesResult.HasNextPage.Should().BeTrue();
        tablesResult.Items.Count.Should().Be(15);
        tablesResult.TotalCount.Should().Be(45);
        tablesResult.TotalPages.Should().Be(3);
    }

    [Fact]
    public async Task GetAllTablesByTableGroup_Should_Return_StatusCode_404_NotFound_When_TableGroupName_Is_Invalid()
    {
        const string restaurantName = "Terry's Steak And Lobster Buffet";
        const string tableGroupName = "Introverts";

        var restaurantId = await CreateRestaurantForRetrieval(restaurantName);
        
        var tablesResponse = await Client.GetAsync($"/api/restaurants/{restaurantId}/tables/{tableGroupName}",
            TestContext.Current.CancellationToken);

        tablesResponse.IsSuccessStatusCode.Should().BeFalse();
        tablesResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}