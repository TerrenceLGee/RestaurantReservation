using System.Net;
using System.Net.Http.Json;

using FluentAssertions;

using RestaurantReservation.Application.Features.Restaurants.Command.Add;
using RestaurantReservation.Application.Features.Restaurants.Query.Response;
using RestaurantReservation.Domain.Restaurants;

namespace RestaurantReservation.IntegrationTests.Restaurants;

public class AddRestaurantTests(IntegrationTestWebAppFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task
        AddRestaurant_Should_Return_RestaurantDetailResponse_And201Created_When_NewRestaurant_SuccessfullyAdded()
    {
        await LoginAndSetAuthenticationAsync("admin@example.com", "Pa$$w0rd");

        var restaurantName = "International House Of Culinary Delights";
        
        var restaurantResponse = await Client.PostAsJsonAsync("/api/restaurants/add", new
        {
            Name = restaurantName,
            Schedule = new RestaurantSchedule[]
            {
                new(WeekDay.Sunday, [new TimeOnly(11, 0), new TimeOnly(21, 00)]),
                new(WeekDay.Monday, [new TimeOnly(9, 00), new TimeOnly(21, 00)]),
                new(WeekDay.Tuesday, [new TimeOnly(9, 0), new TimeOnly(21, 00)]),
                new(WeekDay.Wednesday, [new TimeOnly(9, 0), new TimeOnly(21, 00)]),
                new(WeekDay.Thursday, [new TimeOnly(9, 0), new TimeOnly(22, 0)]),
                new(WeekDay.Friday, [new TimeOnly(9, 0), new TimeOnly(22, 30)]),
                new(WeekDay.Saturday, [new TimeOnly(9, 0), new TimeOnly(23, 00)])
            },
            TableInfo = new TableInfo[]
            {
                new(45, 6, "Main Dining Room"),
                new(30, 4, "Outside Patio"),
                new(10, 3),
                new(20, 8, "Family Dining Room")
            }
        },
            TestContext.Current.CancellationToken);

        var restaurantResult = await
            restaurantResponse.Content.ReadFromJsonAsync<RestaurantAddedResponse>(
                TestContext.Current.CancellationToken);

        restaurantResponse.IsSuccessStatusCode.Should().BeTrue();
        restaurantResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        restaurantResult.Should().NotBeNull();
        restaurantResult.Name.Should().Be(restaurantName);
        restaurantResult.Schedule.Count.Should().Be(7);
        restaurantResult.TableGroups.Count.Should().Be(3);
        restaurantResult.TotalTables.Should().Be(105);
    }

    [Fact]
    public async Task AddRestaurant_Should_Return_401Unauthorized_WhenUnauthenticatedUserTries_ToAddRestaurant()
    {
        var restaurantName = "The Original International House Of Culinary Delights";
        
        var restaurantResponse = await Client.PostAsJsonAsync("/api/restaurants/add", new
            {
                Name = restaurantName,
                Schedule = new RestaurantSchedule[]
                {
                    new(WeekDay.Sunday, [new TimeOnly(11, 0), new TimeOnly(21, 00)]),
                    new(WeekDay.Monday, [new TimeOnly(9, 00), new TimeOnly(21, 00)]),
                    new(WeekDay.Tuesday, [new TimeOnly(9, 0), new TimeOnly(21, 00)]),
                    new(WeekDay.Wednesday, [new TimeOnly(9, 0), new TimeOnly(21, 00)]),
                    new(WeekDay.Thursday, [new TimeOnly(9, 0), new TimeOnly(22, 0)]),
                    new(WeekDay.Friday, [new TimeOnly(9, 0), new TimeOnly(22, 30)]),
                    new(WeekDay.Saturday, [new TimeOnly(9, 0), new TimeOnly(23, 00)])
                },
                TableInfo = new TableInfo[]
                {
                    new(45, 6, "Main Dining Room"),
                    new(30, 4, "Outside Patio"),
                    new(10, 3),
                    new(20, 8, "Family Dining Room")
                }
            },
            TestContext.Current.CancellationToken);

        var restaurantResult = await
            restaurantResponse.Content.ReadFromJsonAsync<RestaurantAddedResponse>(
                TestContext.Current.CancellationToken);

        restaurantResponse.IsSuccessStatusCode.Should().BeFalse();
        restaurantResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        restaurantResult.Should().NotBeNull();
    }
}