namespace RestaurantReservation.Application.Features.Restaurants.Query.Response;

public record RestaurantResponse(
    Guid Id,
    string Name,
    int NumberOfTables);