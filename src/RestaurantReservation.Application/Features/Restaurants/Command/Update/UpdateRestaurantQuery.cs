using RestaurantReservation.Domain.Restaurants;

namespace RestaurantReservation.Application.Features.Restaurants.Command.Update;

public record UpdateRestaurantQuery(string? Name, RestaurantSchedule[]? Schedule);