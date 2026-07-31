using MediatR;

using RestaurantReservation.Application.Features.Restaurants.Query.Response;
using RestaurantReservation.Domain.Common;
using RestaurantReservation.Domain.Restaurants;

namespace RestaurantReservation.Application.Features.Restaurants.Command.Add;

public record AddRestaurantCommand(string Name, RestaurantSchedule[] Schedule, TableInfo[] TableInfo) : IRequest<Result<RestaurantAddedResponse>>;