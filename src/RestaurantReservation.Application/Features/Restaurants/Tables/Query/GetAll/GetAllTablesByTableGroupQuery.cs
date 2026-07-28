using MediatR;

using RestaurantReservation.Application.Features.Restaurants.Tables.Query.Responses;
using RestaurantReservation.Domain.Common;

namespace RestaurantReservation.Application.Features.Restaurants.Tables.Query.GetAll;

public record GetAllTablesByTableGroupQuery(
    Guid RestaurantId, 
    string TableGroupName,
    int Page = 1,
    int PageSize = 10,
    string? SortBy = null) 
    : IRequest<Result<PagedResult<TableResponse>>>;