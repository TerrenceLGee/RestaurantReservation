using MediatR;

namespace RestaurantReservation.Domain.Reservations.Events;

public record ReservationCompletedEvent(
    Guid ReservationId,
    string FirstName,
    string LastName,
    string Email,
    string RestaurantName,
    DateOnly ReservationDate,
    TimeOnly ReservationStartTime,
    TimeOnly ReservationEndTime) : INotification;