using MediatR;

using RestaurantReservation.Application.Features.Reservations.Query.Responses;
using RestaurantReservation.Domain.Common;

namespace RestaurantReservation.Application.Features.Reservations.Command.Reschedule;

public record RescheduleReservationCommand(
    Guid ReservationId,
    string RestaurantName,
    DateOnly RescheduleDate,
    TimeOnly RescheduleStartTime,
    TimeOnly RescheduleEndTime,
    int RescheduleNumberOfGuests) : IRequest<Result<ReservationDetailResponse>>;