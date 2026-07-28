using MediatR;

using RestaurantReservation.Domain.Common;

namespace RestaurantReservation.Application.Features.Restaurants.TableGroups.Command.Delete;

public record DeleteTableGroupCommand(Guid Id, Guid RestaurantId) : IRequest<Result>;