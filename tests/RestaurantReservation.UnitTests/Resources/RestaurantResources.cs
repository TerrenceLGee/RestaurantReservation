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

    public static List<Restaurant> GetRestaurantsToAdd()
    {
        var commands = InitializeRestaurants();
        return [.. commands.Select(GetRestaurantToAdd)];
    }

    public static (Restaurant restaurant, Table table) GetTableToAdd(int seatsAtTable, string? tableGroupName = null)
    {
        var command = AddRestaurant();

        var restaurant = GetRestaurantToAdd(command);

        var table = restaurant.AddTable(seatsAtTable);

        if (!string.IsNullOrEmpty(tableGroupName)) restaurant.AddTableToTableGroup(table, tableGroupName);

        return (restaurant, table);
    }

    public static (Restaurant restaurant, TableGroup tableGroup, List<Table> tables) GetTableGroupToAdd(
        string groupName,
        int numberOfTables,
        int seatsAtTable)
    {
        var command = AddRestaurant();

        var restaurant = GetRestaurantToAdd(command);

        var tables = new List<Table>();

        for (int i = 0; i < numberOfTables; i++)
        {
            tables.Add(restaurant.AddTable(seatsAtTable));
        }

        var tableGroup = restaurant.AddTableGroup(groupName, tables);

        return (restaurant, tableGroup, tables);
    }

    private static List<AddRestaurantCommand> InitializeRestaurants()
    {
        var initializers = new List<AddRestaurantCommand>
        {
            new(
                "Red Lobster",
                [
                new RestaurantSchedule(WeekDay.Sunday, [new TimeOnly(11, 0), new TimeOnly(21, 00)]),
                new RestaurantSchedule(WeekDay.Monday, [new TimeOnly(9, 0), new TimeOnly(21, 00)]),
                new RestaurantSchedule(WeekDay.Tuesday, [new TimeOnly(9, 0), new TimeOnly(21, 00)]),
                new RestaurantSchedule(WeekDay.Wednesday, [new TimeOnly(9, 0), new TimeOnly(21, 00)]),
                new RestaurantSchedule(WeekDay.Thursday, [new TimeOnly(9, 0), new TimeOnly(22, 0)]),
                new RestaurantSchedule(WeekDay.Friday, [new TimeOnly(9, 0), new TimeOnly(22, 30)]),
                new RestaurantSchedule(WeekDay.Saturday, [new TimeOnly(9, 0), new TimeOnly(23, 00)])
            ], 
                [
                new TableInfo(12, 4, "VIP Section"), 
                new TableInfo(30, 6, "Family Dining Room"), 
                new TableInfo(50, 4, "Main Dining Room"),
                new TableInfo(40, 4, "Outside Patio")
            ]),
            new("Longhorn's Steakhouse", 
                [
                new RestaurantSchedule(WeekDay.Sunday, [new TimeOnly(12,0), new TimeOnly(20,0)]),
                new RestaurantSchedule(WeekDay.Monday, [new TimeOnly(12,0), new TimeOnly(22,00)]),
                new RestaurantSchedule(WeekDay.Tuesday, [new TimeOnly(12,0), new TimeOnly(22,00)]),
                new RestaurantSchedule(WeekDay.Wednesday, [new TimeOnly(12,0), new TimeOnly(22,00)]),
                new RestaurantSchedule(WeekDay.Thursday, [new TimeOnly(12,0), new TimeOnly(22,00)]),
                new RestaurantSchedule(WeekDay.Friday, [new TimeOnly(12,0), new TimeOnly(23,00)]),
                new RestaurantSchedule(WeekDay.Saturday, [new TimeOnly(12,0), new TimeOnly(23,00)])],
                [
                    new TableInfo(45, 4, "Non-Smoking"), 
                    new TableInfo(30, 6, "Smoking"), 
                    new TableInfo(30, 4, "Outside Patio")
                ]),
            new("O'Charley's", 
            [
                new RestaurantSchedule(WeekDay.Sunday, [new TimeOnly(12,0), new TimeOnly(21,0)]),
                new RestaurantSchedule(WeekDay.Monday, [new TimeOnly(12,0), new TimeOnly(22,30)]),
                new RestaurantSchedule(WeekDay.Tuesday, [new TimeOnly(11,0), new TimeOnly(22,30)]),
                new RestaurantSchedule(WeekDay.Wednesday, [new TimeOnly(11,0), new TimeOnly(22,30)]),
                new RestaurantSchedule(WeekDay.Thursday, [new TimeOnly(11,0), new TimeOnly(22,30)]),
                new RestaurantSchedule(WeekDay.Friday, [new TimeOnly(11,0), new TimeOnly(22,30)]),
                new RestaurantSchedule(WeekDay.Saturday, [new TimeOnly(11,0), new TimeOnly(23,0)])], 
                [
                new TableInfo(30, 4, "VIP Section"), 
                new TableInfo(90, 4, "Main Dining Room"),
                new TableInfo(30, 4, "Outside Patio")
            ]),
            new("Outback Steakhouse", 
            [
                new RestaurantSchedule(WeekDay.Sunday, [new TimeOnly(11,0), new TimeOnly(21,30)]),
                new RestaurantSchedule(WeekDay.Monday, [new TimeOnly(10,0), new TimeOnly(22,0)]),
                new RestaurantSchedule(WeekDay.Tuesday, [new TimeOnly(10,0), new TimeOnly(22,0)]),
                new RestaurantSchedule(WeekDay.Wednesday, [new TimeOnly(10,0), new TimeOnly(22,0)]),
                new RestaurantSchedule(WeekDay.Thursday, [new TimeOnly(10,0), new TimeOnly(22,0)]),
                new RestaurantSchedule(WeekDay.Friday, [new TimeOnly(10,0), new TimeOnly(22,0)]),
                new RestaurantSchedule(WeekDay.Saturday, [new TimeOnly(10,0), new TimeOnly(22,0)]),
            ], [
                new TableInfo(60, 4, "Main Dining Room")
            ]),
            new("Miller's Ale House", 
            [
                new RestaurantSchedule(WeekDay.Sunday, [null, null]),
                new RestaurantSchedule(WeekDay.Monday, [new TimeOnly(13,0), new TimeOnly(22,0)]),
                new RestaurantSchedule(WeekDay.Tuesday, [new TimeOnly(13,0), new TimeOnly(22,0)]),
                new RestaurantSchedule(WeekDay.Wednesday, [new TimeOnly(13,0), new TimeOnly(22,0)]),
                new RestaurantSchedule(WeekDay.Thursday, [new TimeOnly(13,0), new TimeOnly(22,0)]),
                new RestaurantSchedule(WeekDay.Friday, [new TimeOnly(13,0), new TimeOnly(22,0)]),
                new RestaurantSchedule(WeekDay.Saturday, [new TimeOnly(12,0), new TimeOnly(23,0)]),
            ], [
                new TableInfo(30, 6, "Family Dining Room"), 
                new TableInfo(90, 4, "Main Dining Room"),
                new TableInfo(30, 4, "Outside Patio")
            ]),
            new("Shoney's", 
                [
                new RestaurantSchedule(WeekDay.Sunday, [null, null]),
                new RestaurantSchedule(WeekDay.Monday, [new TimeOnly(7,0), new TimeOnly(22,0)]),
                new RestaurantSchedule(WeekDay.Tuesday, [new TimeOnly(7,0), new TimeOnly(22,0)]),
                new RestaurantSchedule(WeekDay.Wednesday, [new TimeOnly(7,0), new TimeOnly(22,0)]),
                new RestaurantSchedule(WeekDay.Thursday, [new TimeOnly(7,0), new TimeOnly(22,0)]),
                new RestaurantSchedule(WeekDay.Friday, [new TimeOnly(7,0), new TimeOnly(22,0)]),
                new RestaurantSchedule(WeekDay.Saturday, [new TimeOnly(7,0), new TimeOnly(22,0)]),
            ],
                [
                    new TableInfo(30, 4, "Kid's Room"), 
                    new TableInfo(120, 4, "Main Dining Room"),
                ])
        };

        return initializers;
    }
}