using MediatR;

using RestaurantReservation.Domain.Common;

namespace RestaurantReservation.Application.Features.Restaurants.Command.Delete;

public record DeleteRestaurantCommand(Guid Id) : IRequest<Result>;