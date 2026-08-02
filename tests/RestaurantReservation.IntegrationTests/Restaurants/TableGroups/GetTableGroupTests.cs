using System.Net;
using System.Net.Http.Json;

using FluentAssertions;

using RestaurantReservation.Application.Features.Restaurants.TableGroups.Command.Add;
using RestaurantReservation.Application.Features.Restaurants.TableGroups.Query.Responses;

namespace RestaurantReservation.IntegrationTests.Restaurants.TableGroups;

public class GetTableGroupTests(IntegrationTestWebAppFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task GetTableGroup_Returns_TableGroupDetailResponse_And_StatusCode_200OK_OnSuccess()
    {
        const string restaurantName = "Cameron's Desert Factory";
        var restaurantId = await CreateRestaurantForRetrieval(restaurantName);

        const string groupName = "Family Section";

        var tableGroupId = await CreateTableGroupForRetrieval(new AddTableGroupCommand(
            restaurantId,
            groupName,
            20,
            6));

        var tableGroupResponse = await Client.GetAsync($"/api/restaurants/{restaurantId}/tablegroups/{tableGroupId}",
            TestContext.Current.CancellationToken);

        var tableGroupResult = await tableGroupResponse.Content.ReadFromJsonAsync<TableGroupDetailResponse>(
            TestContext.Current.CancellationToken);

        tableGroupResponse.IsSuccessStatusCode.Should().BeTrue();
        tableGroupResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        tableGroupResult.Should().NotBeNull();
        tableGroupResult.GroupName.Should().Be(groupName);
        tableGroupResult.RestaurantName.Should().Be(restaurantName);
        tableGroupResult.NumberOfTables.Should().Be(20);
        tableGroupResult.Tables[0].SeatsAtTable.Should().Be(6);
        tableGroupResult.Tables[0].TableGroupName.Should().Be(groupName);
    }

    [Fact]
    public async Task GetTableGroup_Returns_StatusCode_404NotFound_WhenTableGroupId_IsInvalid()
    {
        const string restaurantName = "Nathan's Desert Bar";

        var restaurantId = await CreateRestaurantForRetrieval(restaurantName);

        var tableGroupId = Guid.CreateVersion7();

        var tableGroupResponse = await Client.GetAsync($"/api/restaurants/{restaurantId}/tablegroups/{tableGroupId}",
            TestContext.Current.CancellationToken);

        tableGroupResponse.IsSuccessStatusCode.Should().BeFalse();
        tableGroupResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}