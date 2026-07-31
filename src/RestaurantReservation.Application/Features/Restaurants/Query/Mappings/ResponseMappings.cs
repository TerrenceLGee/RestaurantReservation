using RestaurantReservation.Application.Features.Restaurants.Query.Response;
using RestaurantReservation.Domain.Common;
using RestaurantReservation.Domain.Restaurants;
using RestaurantReservation.Domain.Tables;

namespace RestaurantReservation.Application.Features.Restaurants.Query.Mappings;

public static class ResponseMappings
{
    public static RestaurantDetailResponse ToDetailResponse(
        this Restaurant restaurant,
        List<Table> tables,
        int totalTableCount,
        int tablePage,
        int tablePageSize)
    {
        return new RestaurantDetailResponse(
            restaurant.Id,
            restaurant.Name,
            restaurant.TableGroups.ToTableGroupNameResponse(),
            restaurant.Schedule.ToScheduleResponseCollection(),
            tables.ToPagedTableResponseCollection(
                totalTableCount, 
                tablePage, 
                tablePageSize));
    }

    public static RestaurantAddedResponse ToAddedDetailResponse(this Restaurant restaurant)
    {
        return new RestaurantAddedResponse(
            restaurant.Id,
            restaurant.Name,
            restaurant.Tables.Count,
            restaurant.TableGroups.ToTableGroupNameResponse(),
            restaurant.Schedule.ToScheduleResponseCollection());
    }

    public static RestaurantResponse ToResponse(this Restaurant restaurant)
    {
        return new RestaurantResponse(
            restaurant.Id,
            restaurant.Name,
            restaurant.Tables.Count);
    }

    public static List<RestaurantResponse> ToResponseCollection(IEnumerable<Restaurant> restaurants)
    {
        return [.. restaurants.Select(r => r.ToResponse())];
    }
    
    public static List<RestaurantScheduleResponse> ToScheduleResponseCollection(
        this IEnumerable<RestaurantSchedule> schedules)
    {
        return [.. schedules.Select(s => s.ToScheduleResponse())];
    }
    
    public static List<RestaurantTableResponse> ToTableResponseCollection(this IEnumerable<Table> tables)
    {
        return [.. tables.Select(t => t.ToTableResponse()).OrderBy(t => t.TableGroupName)];
    }

    public static PagedResult<RestaurantTableResponse> ToPagedTableResponseCollection(
        this List<Table> tables,
        int totalTableCount,
        int tablePage,
        int tablePageSize)
    {
        var tableItems = tables.Select(t => t.ToTableResponse())
            .ToList();

        return new PagedResult<RestaurantTableResponse>(
            tableItems,
            totalTableCount,
            tablePage,
            tablePageSize);
    }
    
    public static RestaurantScheduleResponse ToScheduleResponse(this RestaurantSchedule schedule)
    {
        return new RestaurantScheduleResponse(
            schedule.Day.ToString(),
            schedule.DailyHours);
    }
    
    public static RestaurantTableResponse ToTableResponse(this Table table)
    {
        var groupName = table.TableGroupName ?? "None";

        return new RestaurantTableResponse(
            table.Id,
            table.SeatsAtTable,
            groupName);
    }

    public static List<string> ToTableGroupNameResponse(this IEnumerable<TableGroup> tableGroups)
    {
        return [.. tableGroups.Select(tg => tg.Name).ToList()];
    }
}