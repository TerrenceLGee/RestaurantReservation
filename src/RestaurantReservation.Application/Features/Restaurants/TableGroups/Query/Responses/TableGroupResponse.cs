namespace RestaurantReservation.Application.Features.Restaurants.TableGroups.Query.Responses;

public record TableGroupResponse(Guid Id, Guid RestaurantId, string RestaurantName, string GroupName, int NumberOfTables);