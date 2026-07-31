namespace RestaurantReservation.Application.Features.Restaurants.Query.Response;

public record RestaurantAddedResponse(
    Guid Id,
    string Name,
    int TotalTables,
    List<string> TableGroups,
    List<RestaurantScheduleResponse> Schedule);