using RestaurantReservation.Application.Features.Restaurants.Tables.Query.Responses;
using RestaurantReservation.Domain.Tables;

namespace RestaurantReservation.Application.Features.Restaurants.Tables.Query.Mappings;

public static class ResponseMappings
{
    public static TableDetailResponse ToDetailResponse(this Table table)
    {
        return new TableDetailResponse(
            table.Id,
            table.RestaurantId,
            table.Restaurant?.Name ?? "N/A",
            table.SeatsAtTable,
            table.IsInTableGroup,
            table.TableGroupName ?? "Not in a table group");
    }

    public static TableResponse ToResponse(this Table table)
    {
        return new TableResponse(
            table.Id,
            table.RestaurantId,
            table.Restaurant?.Name ?? "N/A",
            table.SeatsAtTable);
    }

    public static List<TableResponse> ToResponseCollection(this IEnumerable<Table> tables)
    {
        return [.. tables.Select(t => t.ToResponse())];
    }
}