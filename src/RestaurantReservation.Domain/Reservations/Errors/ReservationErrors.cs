using RestaurantReservation.Domain.Common;

namespace RestaurantReservation.Domain.Reservations.Errors;

public static class ReservationErrors
{
    public static DomainError ReservationInvalidCancellationStatus(ReservationStatus status) => new(
        "Cancellation.Invalid",
        $"Cannot cancel reservation with status: {status.ToString()}",
        ErrorType.Validation);

    public static DomainError ReservationInvalidCompletionStatus(ReservationStatus status) => new(
        "Completion.Invalid",
        $"Cannot complete reservation with status: {status.ToString()}",
        ErrorType.Validation);

    public static DomainError ReservationNotFound(string customerEmail, string restaurantName) => new(
        "Reservation.NotFound",
        $"Reservation for {customerEmail} at {restaurantName} not found",
        ErrorType.NotFound);

    public static readonly DomainError ReservationOverlap = new(
        "Reservation.Overlap",
        "This reservation overlaps with an already existing reservation",
        ErrorType.Conflict);

    public static readonly DomainError ReservationCannotBeRescheduled = new(
        "Reservation.CannotBeRescheduled",
        "Unable to reschedule your reservation",
        ErrorType.BadRequest);

    public static readonly DomainError UnableToSecureTablesForReservation = new(
        "Reservation.UnableToSecureTableForReservation",
        "There were no tables available that would fit your party size",
        ErrorType.NotFound);
}