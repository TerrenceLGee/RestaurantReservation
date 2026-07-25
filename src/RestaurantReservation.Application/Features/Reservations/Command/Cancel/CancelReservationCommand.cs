using MediatR;

using RestaurantReservation.Application.Features.Reservations.Query.Responses;
using RestaurantReservation.Domain.Common;

namespace RestaurantReservation.Application.Features.Reservations.Command.Cancel;

public record CancelReservationCommand(
    Guid ReservationId,
    string RestaurantName,
    DateOnly ReservationDate,
    TimeOnly ReservationStartTime,
    TimeOnly ReservationEndTime) : IRequest<Result<ReservationDetailResponse>>;