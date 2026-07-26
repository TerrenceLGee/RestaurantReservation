using MediatR;

using RestaurantReservation.Domain.Common;

namespace RestaurantReservation.Application.Features.Restaurants.Tables.Command.Delete;

public record DeleteTableCommand(Guid RestaurantId, Guid TableId) : IRequest<Result>;