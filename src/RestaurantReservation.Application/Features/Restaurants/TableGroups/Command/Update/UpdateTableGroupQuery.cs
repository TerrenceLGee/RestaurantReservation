namespace RestaurantReservation.Application.Features.Restaurants.TableGroups.Command.Update;

public record UpdateTableGroupQuery(string? GroupName, int? NumberOfTablesToAdd, int? NumberOfSeats);