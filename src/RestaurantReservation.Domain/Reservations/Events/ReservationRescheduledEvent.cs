using MediatR;

namespace RestaurantReservation.Domain.Reservations.Events;

public record ReservationRescheduledEvent(
    Guid ReservationId,
    string FirstName,
    string LastName,
    string Email,
    string RestaurantName,
    DateOnly OriginalReservationDate,
    TimeOnly OriginalReservationStartTime,
    TimeOnly OriginalReservationEndTime,
    DateOnly ReservationDate,
    TimeOnly ReservationStartTime,
    TimeOnly ReservationEndTime,
    int NumberOfGuests) : INotification;