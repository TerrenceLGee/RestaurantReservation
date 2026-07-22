using RestaurantReservation.Domain.Restaurants;
using RestaurantReservation.Domain.Tables;

namespace RestaurantReservation.Infrastructure.Persistence.Seeding;

public static class SeedingResources
{
    public static List<Restaurant> GetRestaurantsForSeeding()
    {
        var restaurants = new List<Restaurant>();

        var restaurant1 = Restaurant.Create("Red Lobster");
        restaurant1.SetSchedule([
            new RestaurantSchedule(WeekDay.Sunday, [new TimeOnly(11, 0), new TimeOnly(21, 00)]),
            new RestaurantSchedule(WeekDay.Monday, [new TimeOnly(9, 00), new TimeOnly(21, 00)]),
            new RestaurantSchedule(WeekDay.Tuesday, [new TimeOnly(9, 0), new TimeOnly(21, 00)]),
            new RestaurantSchedule(WeekDay.Wednesday, [new TimeOnly(9, 0), new TimeOnly(21, 00)]),
            new RestaurantSchedule(WeekDay.Thursday, [new TimeOnly(9, 0), new TimeOnly(22, 0)]),
            new RestaurantSchedule(WeekDay.Friday, [new TimeOnly(9, 0), new TimeOnly(22, 30)]),
            new RestaurantSchedule(WeekDay.Saturday, [new TimeOnly(9, 0), new TimeOnly(23, 00)])
        ]);

        var restaurant2 = Restaurant.Create("Longhorn's Steakhouse");
        restaurant2.SetSchedule([
            new RestaurantSchedule(WeekDay.Sunday, [new TimeOnly(12,0), new TimeOnly(20,0)]),
            new RestaurantSchedule(WeekDay.Monday, [new TimeOnly(12,0), new TimeOnly(22,00)]),
            new RestaurantSchedule(WeekDay.Tuesday, [new TimeOnly(12,0), new TimeOnly(22,00)]),
            new RestaurantSchedule(WeekDay.Wednesday, [new TimeOnly(12,0), new TimeOnly(22,00)]),
            new RestaurantSchedule(WeekDay.Thursday, [new TimeOnly(12,0), new TimeOnly(22,00)]),
            new RestaurantSchedule(WeekDay.Friday, [new TimeOnly(12,0), new TimeOnly(23,00)]),
            new RestaurantSchedule(WeekDay.Saturday, [new TimeOnly(12,0), new TimeOnly(23,00)])]);

        var restaurant3 = Restaurant.Create("O'Charley's");
        restaurant3.SetSchedule( [
            new RestaurantSchedule(WeekDay.Sunday, [new TimeOnly(12,0), new TimeOnly(21,0)]),
            new RestaurantSchedule(WeekDay.Monday, [new TimeOnly(12,0), new TimeOnly(22,30)]),
            new RestaurantSchedule(WeekDay.Tuesday, [new TimeOnly(11,0), new TimeOnly(22,30)]),
            new RestaurantSchedule(WeekDay.Wednesday, [new TimeOnly(11,0), new TimeOnly(22,30)]),
            new RestaurantSchedule(WeekDay.Thursday, [new TimeOnly(11,0), new TimeOnly(22,30)]),
            new RestaurantSchedule(WeekDay.Friday, [new TimeOnly(11,0), new TimeOnly(22,30)]),
            new RestaurantSchedule(WeekDay.Saturday, [new TimeOnly(11,0), new TimeOnly(23,0)])]);

        var restaurant4 = Restaurant.Create("Outback Steakhouse");
        restaurant4.SetSchedule([
            new RestaurantSchedule(WeekDay.Sunday, [new TimeOnly(11,0), new TimeOnly(21,30)]),
            new RestaurantSchedule(WeekDay.Monday, [new TimeOnly(10,0), new TimeOnly(22,0)]),
            new RestaurantSchedule(WeekDay.Tuesday, [new TimeOnly(10,0), new TimeOnly(22,0)]),
            new RestaurantSchedule(WeekDay.Wednesday, [new TimeOnly(10,0), new TimeOnly(22,0)]),
            new RestaurantSchedule(WeekDay.Thursday, [new TimeOnly(10,0), new TimeOnly(22,0)]),
            new RestaurantSchedule(WeekDay.Friday, [new TimeOnly(10,0), new TimeOnly(22,0)]),
            new RestaurantSchedule(WeekDay.Saturday, [new TimeOnly(10,0), new TimeOnly(22,0)]),
        ]);

        var restaurant5 = Restaurant.Create("Miller's Ale House");
        restaurant5.SetSchedule([
            new RestaurantSchedule(WeekDay.Sunday, [null, null]),
            new RestaurantSchedule(WeekDay.Monday, [new TimeOnly(13,0), new TimeOnly(22,0)]),
            new RestaurantSchedule(WeekDay.Tuesday, [new TimeOnly(13,0), new TimeOnly(22,0)]),
            new RestaurantSchedule(WeekDay.Wednesday, [new TimeOnly(13,0), new TimeOnly(22,0)]),
            new RestaurantSchedule(WeekDay.Thursday, [new TimeOnly(13,0), new TimeOnly(22,0)]),
            new RestaurantSchedule(WeekDay.Friday, [new TimeOnly(13,0), new TimeOnly(22,0)]),
            new RestaurantSchedule(WeekDay.Saturday, [new TimeOnly(12,0), new TimeOnly(23,0)]),
        ]);
        
        var restaurant6 = Restaurant.Create("Shoney's");
        restaurant6.SetSchedule([
            new RestaurantSchedule(WeekDay.Sunday, [null, null]),
            new RestaurantSchedule(WeekDay.Monday, [new TimeOnly(7,0), new TimeOnly(22,0)]),
            new RestaurantSchedule(WeekDay.Tuesday, [new TimeOnly(7,0), new TimeOnly(22,0)]),
            new RestaurantSchedule(WeekDay.Wednesday, [new TimeOnly(7,0), new TimeOnly(22,0)]),
            new RestaurantSchedule(WeekDay.Thursday, [new TimeOnly(7,0), new TimeOnly(22,0)]),
            new RestaurantSchedule(WeekDay.Friday, [new TimeOnly(7,0), new TimeOnly(22,0)]),
            new RestaurantSchedule(WeekDay.Saturday, [new TimeOnly(7,0), new TimeOnly(22,0)]),
        ]);
        
        AddTablesToRestaurant(restaurant1, 45);
        AddTablesToRestaurant(restaurant2, 40);
        AddTablesToRestaurant(restaurant3, 36);
        AddTablesToRestaurant(restaurant4, 28);
        AddTablesToRestaurant(restaurant5, 57);
        AddTablesToRestaurant(restaurant6, 30);

        var restaraunt1TableGroupInfo = new Dictionary<int, string?>
        {
            { 20, null }, 
            { 15, "Main Dining Room" }, 
            { 10, null }
        };
        AddTablesToTableGroup(restaurant1, restaraunt1TableGroupInfo);

        var restaurant2TableGroupInfo = new Dictionary<int, string?>
        {
            { 15, "VIP Room" }, 
            { 20, "Main Dining Room" }, { 5, null }
        };
        AddTablesToTableGroup(restaurant2, restaurant2TableGroupInfo);

        var restaurant3TableGroupInfo = new Dictionary<int, string?>
        {
            { 26, null }, 
            { 10, "Smoking Section" }
        };
        AddTablesToTableGroup(restaurant3, restaurant3TableGroupInfo);

        var restaurant4TableGroupInfo = new Dictionary<int, string?>
        {
            { 15, null }, 
            { 13, "Non Smoking Section" }
        };
        AddTablesToTableGroup(restaurant4, restaurant4TableGroupInfo);

        var restarant5TableGroupInfo = new Dictionary<int, string?>
        {
            { 30, null }, 
            { 15, "VIP Room" }, 
            { 12, null }
        };
        AddTablesToTableGroup(restaurant5, restarant5TableGroupInfo);

        var restaurant6TableGroupInfo = new Dictionary<int, string?>
        {
            { 6, "Special Guests Area" }, 
            { 24, null }
        };
        AddTablesToTableGroup(restaurant6, restaurant6TableGroupInfo);
        
        restaurants.Add(restaurant1);
        restaurants.Add(restaurant2);
        restaurants.Add(restaurant3);
        restaurants.Add(restaurant4);
        restaurants.Add(restaurant5);
        restaurants.Add(restaurant6);


        return restaurants;
    }

    private static void AddTablesToRestaurant(Restaurant restaurant, int numberOfTables)
    {
        for (int i = 0; i < numberOfTables; i++)
        {
            var seatsAtTable = Random.Shared.Next(1, 9);
            restaurant.AddTable(seatsAtTable);
        }
    }

    private static void AddTablesToTableGroup(Restaurant restaurant, Dictionary<int, string?> tableInfo)
    {
        var startingIndex = 0;
        var runningCount = 0;
        var stopIndex = restaurant.Tables.Count;
        
        foreach (var info in tableInfo)
        {
            var count = 0;
            var tablesInGroup = info.Key;
            var groupTitle = info.Value;
            var tablesToAdd = new List<RestaurantTable>();
            if (string.IsNullOrEmpty(groupTitle))
            {
                runningCount += tablesInGroup;
                continue;
            }

            while (count <= tablesInGroup)
            {
                tablesToAdd.Add(restaurant.Tables.ElementAt(runningCount++));
                count++;
            }

            restaurant.AddTableGroup(groupTitle, tablesToAdd);

            if (runningCount == stopIndex) break;
        }
    }
}