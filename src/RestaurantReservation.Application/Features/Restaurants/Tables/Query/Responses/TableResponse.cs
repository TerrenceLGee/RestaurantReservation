namespace RestaurantReservation.Application.Features.Restaurants.Tables.Query.Responses;

public record TableResponse(
    Guid Id,
    Guid RestaurantId,
    string RestaurantName,
    int SeatsAtTable);