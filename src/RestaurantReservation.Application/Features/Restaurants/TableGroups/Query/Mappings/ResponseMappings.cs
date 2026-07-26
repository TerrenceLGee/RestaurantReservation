using RestaurantReservation.Application.Features.Restaurants.Query.Mappings;
using RestaurantReservation.Application.Features.Restaurants.TableGroups.Query.Responses;
using RestaurantReservation.Domain.Tables;

namespace RestaurantReservation.Application.Features.Restaurants.TableGroups.Query.Mappings;

public static class ResponseMappings
{
    public static TableGroupResponse ToResponse(this TableGroup group)
    {
        return new TableGroupResponse(
            group.Id,
            group.RestaurantId,
            group.Restaurant?.Name ?? "N/A",
            group.Name,
            group.Tables.Count);
    }

    public static TableGroupDetailResponse ToDetailResponse(this TableGroup group)
    {
        return new TableGroupDetailResponse(
            group.Id,
            group.RestaurantId,
            group.Restaurant?.Name ?? "N/A",
            group.Name,
            group.Tables.Count,
            group.Tables.ToTableResponseCollection());
    }

    public static List<TableGroupResponse> ToResponseCollection(this IEnumerable<TableGroup> groups)
    {
        return [.. groups.Select(g => g.ToResponse())];
    }

    public static List<TableGroupDetailResponse> ToDetailResponseCollection(this IEnumerable<TableGroup> groups)
    {
        return [.. groups.Select(g => g.ToDetailResponse())];
    }
}