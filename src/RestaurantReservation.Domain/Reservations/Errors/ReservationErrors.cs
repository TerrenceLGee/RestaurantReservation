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

    public static DomainError ReservationNotFound(string customerId, Guid restaurantId) => new(
        "Reservation.NotFound",
        $"Reservation for customer with id {customerId} at restaurant with id {restaurantId} not found",
        ErrorType.NotFound);

    public static readonly DomainError ReservationOverlap = new(
        "Reservation.Overlap",
        "This reservation overlaps with an already existing reservation",
        ErrorType.Conflict);

    public static readonly DomainError ReservationCannotBeRescheduled = new(
        "Reservation.CannotBeRescheduled",
        "Unable to reschedule your reservation",
        ErrorType.BadRequest);
}