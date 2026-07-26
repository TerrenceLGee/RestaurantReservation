namespace RestaurantReservation.Application.Features.Restaurants.Query.Response;

public record RestaurantDetailResponse(
    Guid Id,
    string Name,
    List<RestaurantScheduleResponse> Schedule,
    List<RestaurantTableResponse> TableInfo);