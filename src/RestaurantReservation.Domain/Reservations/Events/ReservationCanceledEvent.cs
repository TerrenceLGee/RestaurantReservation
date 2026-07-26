using MediatR;

namespace RestaurantReservation.Domain.Reservations.Events;

public record ReservationCanceledEvent(
    Guid ReservationId,
    string FirstName,
    string LastName,
    string Email,
    string RestaurantName,
    DateOnly ReservationDate,
    TimeOnly ReservationStartTime,
    TimeOnly ReservationEndTime,
    DateTime ReservationCanceledAtUtc) : INotification;