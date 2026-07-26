using MediatR;

using RestaurantReservation.Domain.Common;

namespace RestaurantReservation.Application.Features.Restaurants.Tables.Command.Update;

public record UpdateTableCommand(
    Guid RestaurantId, 
    Guid TableId, 
    int? NumberOfSeats, 
    string? GroupName) : IRequest<Result>;