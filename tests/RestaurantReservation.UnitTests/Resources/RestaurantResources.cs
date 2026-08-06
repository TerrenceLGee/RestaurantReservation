using RestaurantReservation.Application.Features.Restaurants.Command.Add;
using RestaurantReservation.Domain.Restaurants;
using RestaurantReservation.Domain.Tables;

namespace RestaurantReservation.UnitTests.Resources;

public static class RestaurantResources
{
    public static AddRestaurantCommand AddRestaurant()
    {
        const string name = "International House Of Culinary Delights";

        var schedule = new RestaurantSchedule[]
        {
            new(WeekDay.Sunday, [new TimeOnly(11, 0), new TimeOnly(21, 00)]),
            new(WeekDay.Monday, [new TimeOnly(9, 0), new TimeOnly(21, 00)]),
            new(WeekDay.Tuesday, [new TimeOnly(9, 0), new TimeOnly(21, 00)]),
            new(WeekDay.Wednesday, [new TimeOnly(9, 0), new TimeOnly(21, 00)]),
            new(WeekDay.Thursday, [new TimeOnly(9, 0), new TimeOnly(22, 0)]),
            new(WeekDay.Friday, [new TimeOnly(9, 0), new TimeOnly(22, 30)]),
            new(WeekDay.Saturday, [new TimeOnly(9, 0), new TimeOnly(23, 00)])
        };

        var tableInfo = new TableInfo[]
        {
            new(12, 4, "VIP Section"), 
            new(30, 6, "Family Dining Room"), 
            new(50, 4, "Main Dining Room"),
            new(40, 4, "Outside Patio")
        };

        return new AddRestaurantCommand(
            name,
            schedule,
            tableInfo);
    }

    public static Restaurant GetRestaurantToAdd(AddRestaurantCommand command)
    {
        var restaurant = Restaurant.Create(command.Name);

        restaurant.SetSchedule([.. command.Schedule]);

        foreach (var info in command.TableInfo)
        {
            var tables = new List<Table>();
            for (int i = 0; i < info.NumberOfTables; i++)
            {
                tables.Add(restaurant.AddTable(info.NumberOfSeats));
            }

            if (!string.IsNullOrEmpty(info.GroupName))
            {
                restaurant.AddTableGroup(info.GroupName, tables);
            }
        }

        return restaurant;
    }
}