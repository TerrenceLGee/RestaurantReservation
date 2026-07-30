namespace RestaurantReservation.Api.Endpoints.Constants;

public static class Restaurant
{
    public static string BaseUri => "/api/restaurants";
    public static string Tag => "Restaurants";
    public static string GetAll => "/";
    public static string Get => "/{id:guid}";
    public static string Add => "/add";
    public static string Update => "/update";
    public static string Delete => "/delete";
    public static string AddTable => "/{id:guid}/tables/add";
    public static string UpdateTable => "/{restaurantId:guid}/tables/{tableId:guid}/update";
}