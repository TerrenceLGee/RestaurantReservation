using MediatR;

using RestaurantReservation.Application.Features.Restaurants.TableGroups.Query.Responses;
using RestaurantReservation.Domain.Common;

namespace RestaurantReservation.Application.Features.Restaurants.TableGroups.Query.Get;

public record GetTableGroupQuery(Guid RestaurantId, Guid TableGroupId) : IRequest<Result<TableGroupDetailResponse>>;