using MediatR;

using RestaurantReservation.Application.Features.Restaurants.Command.Add;
using RestaurantReservation.Application.Features.Restaurants.TableGroups.Query.Responses;
using RestaurantReservation.Domain.Common;

namespace RestaurantReservation.Application.Features.Restaurants.TableGroups.Command.Add;

public record AddTableGroupCommand(Guid RestaurantId, string Name, int? NumberOfTables, int? NumberOfSeats) : IRequest<Result<TableGroupDetailResponse>>;