using MediatR;

using RestaurantReservation.Domain.Common;

namespace RestaurantReservation.Application.Features.Restaurants.TableGroups.Command.Update;

public record UpdateTableGroupCommand(Guid Id, Guid RestaurantId, string? GroupName, int? NumberOfTables, int? SeatsAtTable) : IRequest<Result>;