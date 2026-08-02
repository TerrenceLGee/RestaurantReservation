using System.Net;
using System.Net.Http.Json;

using FluentAssertions;

using RestaurantReservation.Application.Features.Restaurants.TableGroups.Command.Add;
using RestaurantReservation.Application.Features.Restaurants.TableGroups.Query.Responses;

namespace RestaurantReservation.IntegrationTests.Restaurants.TableGroups;

public class UpdateTableGroupTests(IntegrationTestWebAppFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task UpdateTableGroup_Should_Return_StatusCode_204NoContent_OnSuccess()
    {
        var restaurantId = await CreateRestaurantForRetrieval("Charlie's House Of Brownies");
        
        const string groupName = "Smoking Section";
        const string updatedGroupName = "V.I.P. Section";

        var tableGroupId = await CreateTableGroupForRetrieval(new AddTableGroupCommand(
            restaurantId,
            groupName,
            30,
            4));

        var tableGroupResponse = await Client.PutAsJsonAsync($"/api/restaurants/{restaurantId}/tablegroups/update/{tableGroupId}", new
            {
                GroupName = updatedGroupName,
                NumberOfTables = 30,
                NumberOfSeats = 6
            },
            TestContext.Current.CancellationToken);

        var updatedTableGroup = await GetTableGroupToCheckProperties(
            restaurantId, 
            tableGroupId);

        tableGroupResponse.IsSuccessStatusCode.Should().BeTrue();
        tableGroupResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
        updatedTableGroup.GroupName.Should().Be(updatedGroupName);
        updatedTableGroup.Id.Should().Be(tableGroupId);
        updatedTableGroup.NumberOfTables.Should().Be(60);
    }

    [Fact]
    public async Task
        UpdateTableGroup_Should_Return_StatusCode_409Conflict_WhenTryingToUpdateGroupName_ToNameAlreadyTaken()
    {
        var restaurantId = await CreateRestaurantForRetrieval("Ali's House Of Decadent Culinary Delights");

        const string groupName = "Smoking Section";
        const string updatedGroupName = "Main Dining Room";

        var tableGroupId = await CreateTableGroupForRetrieval(new AddTableGroupCommand(
            restaurantId,
            groupName,
            30,
            4));

        var tableGroupResponse = await Client.PutAsJsonAsync(
            $"/api/restaurants/{restaurantId}/tablegroups/update/{tableGroupId}",
            new { GroupName = updatedGroupName, NumberOfTables = 30, NumberOfSeats = 6 },
            TestContext.Current.CancellationToken);

        tableGroupResponse.IsSuccessStatusCode.Should().BeFalse();
        tableGroupResponse.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }
    
    
    private async Task<TableGroupDetailResponse> GetTableGroupToCheckProperties(Guid restaurantId, Guid tableGroupId)
    {
        var tableGroupResponse = await Client.GetAsync($"/api/restaurants/{restaurantId}/tablegroups/{tableGroupId}",
            TestContext.Current.CancellationToken);

        tableGroupResponse.EnsureSuccessStatusCode();

        var tableGroupResult = await tableGroupResponse.Content.ReadFromJsonAsync<TableGroupDetailResponse>(
            TestContext.Current.CancellationToken);

        tableGroupResult.Should().NotBeNull();

        return tableGroupResult;
    }
}