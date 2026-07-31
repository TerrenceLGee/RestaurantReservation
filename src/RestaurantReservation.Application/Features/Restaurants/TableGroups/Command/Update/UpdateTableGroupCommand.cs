using MediatR;

using RestaurantReservation.Domain.Common;

namespace RestaurantReservation.Application.Features.Restaurants.TableGroups.Command.Update;

public record UpdateTableGroupCommand(Guid RestaurantId, Guid TableGroupId, string? GroupName, int? NumberOfTables, int? SeatsAtTable) : IRequest<Result>;