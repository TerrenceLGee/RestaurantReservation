namespace RestaurantReservation.Application.Features.Restaurants.TableGroups.Command.Add;

public record AddTableGroupQuery(string Name, int? NumberOfTables, int? NumberOfSeats);