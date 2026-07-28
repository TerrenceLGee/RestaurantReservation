using MediatR;

using RestaurantReservation.Application.Features.Restaurants.Tables.Query.Responses;
using RestaurantReservation.Domain.Common;

namespace RestaurantReservation.Application.Features.Restaurants.Tables.Query.Get;

public record GetTableQuery(Guid RestaurantId, Guid TableId) : IRequest<Result<TableDetailResponse>>;