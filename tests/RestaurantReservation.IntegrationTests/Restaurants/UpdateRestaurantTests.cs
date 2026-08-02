using System.Net;
using System.Net.Http.Json;

using FluentAssertions;

using RestaurantReservation.Domain.Restaurants;

namespace RestaurantReservation.IntegrationTests.Restaurants;

public class UpdateRestaurantTests(IntegrationTestWebAppFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task UpdateRestaurant_Should_Be_Successful_WhenId_IsValid()
    {
        const string originalName = "Susan's Vegan Health Shack";
        const string updatedName = "Susan's Healthy Vegan Meals And Snacks";
        
        var restaurantId = await CreateRestaurantForRetrieval(originalName);

        var restaurantResponse = await Client.PutAsJsonAsync($"/api/restaurants/update/{restaurantId}", new
        {
            Name = updatedName,
            Schedule = new RestaurantSchedule[]
            {
                new(WeekDay.Sunday, [new TimeOnly(9, 00), new TimeOnly(21,00)]),
                new(WeekDay.Monday, [new TimeOnly(8, 00), new TimeOnly(21, 00)]),
                new(WeekDay.Tuesday, [new TimeOnly(8, 0), new TimeOnly(21, 00)]),
                new(WeekDay.Wednesday, [new TimeOnly(8, 0), new TimeOnly(21, 00)]),
                new(WeekDay.Thursday, [new TimeOnly(8, 0), new TimeOnly(22, 0)]),
                new(WeekDay.Friday, [new TimeOnly(8, 0), new TimeOnly(22, 30)]),
                new(WeekDay.Saturday, [new TimeOnly(9, 0), new TimeOnly(23, 00)])
            }
        }, 
            TestContext.Current.CancellationToken);

        restaurantResponse.IsSuccessStatusCode.Should().BeTrue();
        restaurantResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task UpdateRestaurant_Should_Fail_WhenRestaurantNotFound()
    {
        await LoginAndSetAuthenticationAsync("admin@example.com", "Pa$$w0rd");

        const string updatedName = "Susan's Healthy Vegan Meals And Snacks";
        var restaurantId = Guid.CreateVersion7();
        
        var restaurantResponse = await Client.PutAsJsonAsync($"/api/restaurants/update/{restaurantId}", new
            {
                Name = updatedName,
                Schedule = new RestaurantSchedule[]
                {
                    new(WeekDay.Sunday, [new TimeOnly(9, 00), new TimeOnly(21,00)]),
                    new(WeekDay.Monday, [new TimeOnly(8, 00), new TimeOnly(21, 00)]),
                    new(WeekDay.Tuesday, [new TimeOnly(8, 0), new TimeOnly(21, 00)]),
                    new(WeekDay.Wednesday, [new TimeOnly(8, 0), new TimeOnly(21, 00)]),
                    new(WeekDay.Thursday, [new TimeOnly(8, 0), new TimeOnly(22, 0)]),
                    new(WeekDay.Friday, [new TimeOnly(8, 0), new TimeOnly(22, 30)]),
                    new(WeekDay.Saturday, [new TimeOnly(9, 0), new TimeOnly(23, 00)])
                }
            }, 
            TestContext.Current.CancellationToken);

        restaurantResponse.IsSuccessStatusCode.Should().BeFalse();
        restaurantResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}