namespace RestaurantReservation.Application.Features.Restaurants.Tables.Command.Add;

public record AddTableQuery(int NumberOfSeats, string? TableGroup);