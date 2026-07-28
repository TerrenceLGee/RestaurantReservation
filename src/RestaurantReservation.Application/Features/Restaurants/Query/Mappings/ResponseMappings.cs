using RestaurantReservation.Application.Features.Restaurants.Query.Response;
using RestaurantReservation.Domain.Restaurants;
using RestaurantReservation.Domain.Tables;

namespace RestaurantReservation.Application.Features.Restaurants.Query.Mappings;

public static class ResponseMappings
{
    public static RestaurantDetailResponse ToDetailResponse(this Restaurant restaurant)
    {
        return new RestaurantDetailResponse(
            restaurant.Id,
            restaurant.Name,
            restaurant.Schedule.ToScheduleResponseCollection(),
            restaurant.Tables.ToTableResponseCollection());
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
        return [.. tables.Select(t => t.ToTableResponse())];
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
}