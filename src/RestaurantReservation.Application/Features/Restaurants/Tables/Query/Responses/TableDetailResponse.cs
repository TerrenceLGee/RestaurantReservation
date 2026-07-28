namespace RestaurantReservation.Application.Features.Restaurants.Tables.Query.Responses;

public record TableDetailResponse(
    Guid Id,
    Guid RestaurantId,
    string RestaurantName,
    int SeatsAtTable,
    bool IsInTableGroup,
    string? TableGroupName = null);