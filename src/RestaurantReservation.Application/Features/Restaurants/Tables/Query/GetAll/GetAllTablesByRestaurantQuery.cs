using MediatR;

using RestaurantReservation.Application.Features.Restaurants.Tables.Query.Responses;
using RestaurantReservation.Domain.Common;

namespace RestaurantReservation.Application.Features.Restaurants.Tables.Query.GetAll;

public record GetAllTablesByRestaurantQuery(
    Guid RestaurantId,
    int Page = 1,
    int PageSize = 10,
    string? SortBy = null,
    string? GroupName = null) : IRequest<Result<PagedResult<TableResponse>>>;