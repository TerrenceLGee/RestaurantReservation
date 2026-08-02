using System.Net;

using FluentAssertions;

using RestaurantReservation.Application.Features.Restaurants.TableGroups.Command.Add;

namespace RestaurantReservation.IntegrationTests.Restaurants.TableGroups;

public class DeleteTableGroupTests(IntegrationTestWebAppFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task DeleteTable_Returns_StatusCode_204NoContent_OnSuccess()
    {
        var restaurantId = await CreateRestaurantForRetrieval("Steak Supreme");

        const string groupName = "V.I.P";

        var tableGroupId = await CreateTableGroupForRetrieval(new AddTableGroupCommand(
            restaurantId,
            groupName,
            30,
            6));

        var tableGroupResponse = await Client.DeleteAsync(
            $"/api/restaurants/{restaurantId}/tablegroups/delete/{tableGroupId}",
            TestContext.Current.CancellationToken);

        tableGroupResponse.IsSuccessStatusCode.Should().BeTrue();
        tableGroupResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteTableGroup_Returns_StatusCode_404NotFound_When_TableGroupId_IsInvalid()
    {
        var restaurantId = await CreateRestaurantForRetrieval("The New Steak Supreme");

        var tableGroupId = Guid.CreateVersion7();

        var tableGroupResponse = await Client.DeleteAsync($"/api/restaurants/{restaurantId}/tablegroups/delete/{tableGroupId}",
            TestContext.Current.CancellationToken);

        tableGroupResponse.IsSuccessStatusCode.Should().BeFalse();
        tableGroupResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}