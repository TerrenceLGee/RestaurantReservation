namespace RestaurantReservation.Api.Endpoints.Constants;

public static class Restaurant
{
    public static string BaseUri => "/api/restaurants";
    public static string Tag => "Restaurants";
    public static string GetAll => "/";
    public static string Get => "/{id:guid}";
    public static string Add => "/add";
    public static string Update => "/update/{id:guid}";
    public static string Delete => "/delete/{id:guid}";
    public static string AddTable => "/{id:guid}/tables/add";
    public static string UpdateTable => "/{restaurantId:guid}/tables/update/{tableId:guid}";
    public static string DeleteTable => "/{restaurantId:guid}/tables/delete/{tableId:guid}";
    public static string GetTable => "/{restaurantId:guid}/tables/{tableId:guid}";
    public static string GetTables => "/{restaurantId:guid}/tables";
    public static string GetTablesByGroupName => "/{restaurantId:guid}/tables/{tableGroupName}";
    public static string AddTableGroup => "/{restaurantId:guid}/tablegroups/add";
    public static string UpdateTableGroup => "/{restaurantId:guid}/tablegroups/update/{tableGroupId:guid}";
    public static string DeleteTableGroup => "/{restaurantId:guid}/tablegroups/delete/{tableGroupId:guid}";
    public static string GetTableGroup => "{restaurantId:guid}/tablegroups/{tableGroupId:guid}";
    public static string GetTableGroups => "{restaurantId:guid}/tablegroups";
}