using RestaurantReservation.Domain.Common;

namespace RestaurantReservation.Application.Features.Restaurants.Query.Response;

public record RestaurantDetailResponse(
    Guid Id,
    string Name,
    List<string> TableGroups,
    List<RestaurantScheduleResponse> Schedule,
    PagedResult<RestaurantTableResponse> TableInfo);