using MediatR;

using RestaurantReservation.Application.Features.Restaurants.Query.Response;
using RestaurantReservation.Domain.Common;

namespace RestaurantReservation.Application.Features.Restaurants.Tables.Command.Add;

public record AddTableCommand(Guid RestaurantId, int NumberOfSeats, string? TableGroup) : IRequest<Result<RestaurantTableResponse>>;