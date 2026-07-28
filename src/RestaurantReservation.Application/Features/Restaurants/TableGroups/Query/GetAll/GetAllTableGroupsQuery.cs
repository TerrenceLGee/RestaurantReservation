using MediatR;

using RestaurantReservation.Application.Features.Restaurants.TableGroups.Query.Responses;
using RestaurantReservation.Domain.Common;

namespace RestaurantReservation.Application.Features.Restaurants.TableGroups.Query.GetAll;

public record GetAllTableGroupsQuery(
    Guid RestaurantId,
    int Page = 1,
    int PageSize = 10,
    string? SortBy = null,
    string? Name = null) : IRequest<Result<PagedResult<TableGroupResponse>>>;
