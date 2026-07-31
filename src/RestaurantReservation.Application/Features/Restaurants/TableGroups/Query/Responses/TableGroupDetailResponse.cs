using RestaurantReservation.Application.Features.Restaurants.Query.Response;

namespace RestaurantReservation.Application.Features.Restaurants.TableGroups.Query.Responses;

public record TableGroupDetailResponse(Guid Id, Guid RestaurantId, string RestaurantName, string GroupName, int NumberOfTables, List<RestaurantTableResponse> Tables);