using MediatR;

using RestaurantReservation.Domain.Common;
using RestaurantReservation.Domain.Restaurants;

namespace RestaurantReservation.Application.Features.Restaurants.Command.Update;

public record UpdateRestaurantCommand(Guid Id, string? Name, RestaurantSchedule[]? Schedule) : IRequest<Result>;