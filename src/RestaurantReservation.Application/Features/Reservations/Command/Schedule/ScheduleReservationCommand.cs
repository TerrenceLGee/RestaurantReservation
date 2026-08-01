using MediatR;

using RestaurantReservation.Application.Features.Reservations.Query.Responses;
using RestaurantReservation.Domain.Common;

namespace RestaurantReservation.Application.Features.Reservations.Command.Schedule;

public record ScheduleReservationCommand(
    string RestaurantName,
    string CustomerFirstName,
    string CustomerLastName,
    string CustomerEmail,
    string CustomerPhone,
    DateOnly ReservationDate,
    TimeOnly ReservationStartTime,
    TimeOnly ReservationEndTime,
    int NumberOfGuests) : IRequest<Result<ReservationDetailResponse>>;