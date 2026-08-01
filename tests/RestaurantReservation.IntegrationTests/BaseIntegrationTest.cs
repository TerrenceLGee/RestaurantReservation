using System.Net.Http.Headers;
using System.Net.Http.Json;

using FluentAssertions;

using MediatR;

using Microsoft.Extensions.DependencyInjection;

using RestaurantReservation.Application.Features.Auth.Command.Login;
using RestaurantReservation.Application.Features.Reservations.Query.Responses;
using RestaurantReservation.Application.Features.Restaurants.Command.Add;
using RestaurantReservation.Application.Features.Restaurants.Query.Response;
using RestaurantReservation.Domain.Restaurants;
using RestaurantReservation.Infrastructure.Persistence;

namespace RestaurantReservation.IntegrationTests;

public class BaseIntegrationTest : IClassFixture<IntegrationTestWebAppFactory>
{
    protected readonly IMediator Sender;
    protected readonly ApplicationDbContext Context;
    protected readonly HttpClient Client;

    protected BaseIntegrationTest(IntegrationTestWebAppFactory factory)
    {
        IServiceScope scope = factory.Services.CreateScope();
        Client = factory.CreateClient();
        Sender = scope.ServiceProvider.GetRequiredService<IMediator>();
        Context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    }

    protected async Task LoginAndSetAuthenticationAsync(string email, string password)
    {
        var loginResponse = await Client
            .PostAsJsonAsync("/api/auth/login", new { Email = email, Password = password },
                TestContext.Current.CancellationToken);
        loginResponse.EnsureSuccessStatusCode();

        var loginResult = await loginResponse.Content
            .ReadFromJsonAsync<LoginResponse>(TestContext.Current.CancellationToken);

        Client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", loginResult!.AccessToken);
    }

    protected async Task<Guid> ScheduleReservationForTesting()
    {
        await LoginAndSetAuthenticationAsync("customer@example.com", "Pa$$w0rd");

        var reservationResponse = await Client.PostAsJsonAsync("/api/reservations/schedulereservation",
            new
            {
                RestaurantName = "Red Lobster",
                CustomerFirstName = "Dennis",
                CustomerLastName = "Edwards",
                CustomerEmail = "customer@example.com",
                CustomerPhone = "555-123-4567",
                ReservationDate = new DateOnly(2026, 9, 1),
                ReservationStartTime = new TimeOnly(19, 00),
                ReservationEndTime = new TimeOnly(20, 00),
                NumberOfGuests = 3
            }, TestContext.Current.CancellationToken);

        reservationResponse.EnsureSuccessStatusCode();

        var reservationResult = await
            reservationResponse.Content.ReadFromJsonAsync<ReservationDetailResponse>(
                TestContext.Current.CancellationToken);

        reservationResult.Should().NotBeNull();

        return reservationResult.ReservationId;
    }

    protected async Task<Guid> CreateRestaurantForRetrieval(string name)
    {
        await LoginAndSetAuthenticationAsync("admin@example.com", "Pa$$w0rd");

        var restaurantResponse = await Client.PostAsJsonAsync("/api/restaurants/add",
            new
            {
                Name = name,
                Schedule = new RestaurantSchedule[]
                {
                    new(WeekDay.Sunday, [new TimeOnly(10, 00), new TimeOnly(21, 00)]),
                    new(WeekDay.Monday, [new TimeOnly(9, 00), new TimeOnly(21, 00)]),
                    new(WeekDay.Tuesday, [new TimeOnly(9, 0), new TimeOnly(21, 00)]),
                    new(WeekDay.Wednesday, [new TimeOnly(9, 0), new TimeOnly(21, 00)]),
                    new(WeekDay.Thursday, [new TimeOnly(9, 0), new TimeOnly(22, 0)]),
                    new(WeekDay.Friday, [new TimeOnly(9, 0), new TimeOnly(22, 30)]),
                    new(WeekDay.Saturday, [new TimeOnly(9, 0), new TimeOnly(23, 00)])
                },
                TableInfo = new TableInfo[] { new(45, 6, "Main Dining Room") }
            }, TestContext.Current.CancellationToken);
            
            restaurantResponse.EnsureSuccessStatusCode();

            var restaurantResult = await restaurantResponse.Content.ReadFromJsonAsync<RestaurantDetailResponse>(
                TestContext.Current.CancellationToken);

            restaurantResult.Should().NotBeNull();

            return restaurantResult.Id;
    }
}