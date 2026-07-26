namespace RestaurantReservation.Application.Features.Restaurants.Command.Add;

public record TableInfo(int NumberOfTables, int NumberOfSeats, string? GroupName = null);