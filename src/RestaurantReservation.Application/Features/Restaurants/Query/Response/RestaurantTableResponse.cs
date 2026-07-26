namespace RestaurantReservation.Application.Features.Restaurants.Query.Response;

public record RestaurantTableResponse(Guid Id, int SeatsAtTable, string? TableGroupName = null);