namespace RestaurantReservation.Domain.Reservations.ValueObjects.ReservationValueObjects;

public record ReservationInfo(
    ReservationDate Date,
    ReservationStart StartTime,
    ReservationEnd EndTime,
    GuestsInParty Guests);