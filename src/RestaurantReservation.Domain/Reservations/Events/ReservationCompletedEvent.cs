using MediatR;

namespace RestaurantReservation.Domain.Reservations.Events;

public record ReservationCompletedEvent(
    Guid RestaurantId,
    string FirstName,
    string LastName,
    string Email,
    string RestaurantName,
    DateOnly ReservationDate,
    TimeOnly ReservationStartTime,
    TimeOnly ReservationEndTime,
    int NumberOfGuests) : INotification;