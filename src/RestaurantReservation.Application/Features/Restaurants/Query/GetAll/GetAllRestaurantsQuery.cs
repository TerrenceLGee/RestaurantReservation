using MediatR;

using RestaurantReservation.Application.Features.Restaurants.Query.Response;
using RestaurantReservation.Domain.Common;

namespace RestaurantReservation.Application.Features.Restaurants.Query.GetAll;

public record GetAllRestaurantsQuery(
    int Page = 1,
    int PageSize = 10,
    string? SortBy = null,
    string? Name = null) : IRequest<Result<PagedResult<RestaurantResponse>>>;