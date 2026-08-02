using System.Net;
using System.Net.Http.Json;

using FluentAssertions;

using RestaurantReservation.Application.Features.Restaurants.TableGroups.Query.Responses;

namespace RestaurantReservation.IntegrationTests.Restaurants.TableGroups;

public class AddTableGroupTests(IntegrationTestWebAppFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task AddTableGroup_ShouldReturn_TableGroupDetailResponse_And_StatusCode_201Created_OnSuccess()
    {
        const string restaurantName = "Sal's Seafood Shack";
        const string groupName = "Non-Smoking";
        
        var restaurantId = await CreateRestaurantForRetrieval(restaurantName);

        var tableGroupResponse = await Client.PostAsJsonAsync($"/api/restaurants/{restaurantId}/tablegroups/add", new
        {
            Name = groupName,
            NumberOfTables = 45,
            NumberOfSeats = 4
        }, 
            TestContext.Current.CancellationToken);

        var tableGroupResult = await tableGroupResponse.Content.ReadFromJsonAsync<TableGroupDetailResponse>(
            TestContext.Current.CancellationToken);

        tableGroupResponse.IsSuccessStatusCode.Should().BeTrue();
        tableGroupResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        tableGroupResult.Should().NotBeNull();
        tableGroupResult.GroupName.Should().Be(groupName);
        tableGroupResult.RestaurantName.Should().Be(restaurantName);
        tableGroupResult.RestaurantId.Should().Be(restaurantId);
        tableGroupResult.NumberOfTables.Should().Be(45);
        tableGroupResult.Tables[0].SeatsAtTable.Should().Be(4);
    }

    [Fact]
    public async Task AddTableGroup_Returns_StatusCode_404NotFound_When_RestaurantId_IsNotValid()
    {
        await LoginAndSetAuthenticationAsync("admin@example.com", "Pa$$w0rd");
        
        const string groupName = "Non-Smoking";

        var restaurantId = Guid.CreateVersion7();

        var tableGroupResponse = await Client.PostAsJsonAsync($"/api/restaurants/{restaurantId}/tablegroups/add",
            new { Name = groupName, NumberOfTables = 45, NumberOfSeats = 4 },
            TestContext.Current.CancellationToken);

        tableGroupResponse.IsSuccessStatusCode.Should().BeFalse();
        tableGroupResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}