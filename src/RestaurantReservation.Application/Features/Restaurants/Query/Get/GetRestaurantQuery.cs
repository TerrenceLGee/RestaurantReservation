using MediatR;

using RestaurantReservation.Application.Features.Restaurants.Query.Response;
using RestaurantReservation.Domain.Common;

namespace RestaurantReservation.Application.Features.Restaurants.Query.Get;

public record GetRestaurantQuery(Guid Id) : IRequest<Result<RestaurantDetailResponse>>;