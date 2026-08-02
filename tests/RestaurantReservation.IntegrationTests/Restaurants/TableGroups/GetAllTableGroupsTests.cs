using System.Net;
using System.Net.Http.Json;

using FluentAssertions;

using RestaurantReservation.Application.Features.Restaurants.TableGroups.Query.Responses;
using RestaurantReservation.Domain.Common;

namespace RestaurantReservation.IntegrationTests.Restaurants.TableGroups;

public class GetAllTableGroupsTests(IntegrationTestWebAppFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task GetAllTableGroups_Returns_TableGroups_And_StatusCode_200OK_OnSuccess()
    {
        const string restaurantName = "Guy's Pancake Shack";

        var restaurantId = await CreateRestaurantForRetrieval(restaurantName);

        var tableGroupsResponse = await Client.GetAsync($"/api/restaurants/{restaurantId}/tablegroups",
            TestContext.Current.CancellationToken);

        var tableGroupsResult = await tableGroupsResponse.Content.ReadFromJsonAsync<PagedResult<TableGroupResponse>>(
            TestContext.Current.CancellationToken);

        tableGroupsResponse.IsSuccessStatusCode.Should().BeTrue();
        tableGroupsResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        tableGroupsResult.Should().NotBeNull();
        tableGroupsResult.TotalCount.Should().Be(3);
        tableGroupsResult.HasPreviousPage.Should().BeFalse();
        tableGroupsResult.HasNextPage.Should().BeFalse();
    }
}