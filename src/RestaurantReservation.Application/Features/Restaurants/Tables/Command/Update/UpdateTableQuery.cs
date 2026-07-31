namespace RestaurantReservation.Application.Features.Restaurants.Tables.Command.Update;

public record UpdateTableQuery(int? NumberOfSeats, string? GroupName);