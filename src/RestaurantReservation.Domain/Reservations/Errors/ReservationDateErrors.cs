using RestaurantReservation.Domain.Common;

namespace RestaurantReservation.Domain.Reservations.Errors;

public static class ReservationDateErrors
{
    public static readonly DomainError ReservationDateInvalid = new(
        "ReservationDate.Invalid",
        "A reservation cannot be made for a date in the past",
        ErrorType.Validation);
}