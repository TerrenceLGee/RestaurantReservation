using RestaurantReservation.Domain.Common;

namespace RestaurantReservation.Domain.Reservations.Errors;

public static class ReservationTimeErrors
{
    public static readonly DomainError ReservationTimeInvalid = new(
        "ReservationTime.Invalid",
        "Start time of reservation must come before end time of reservation",
        ErrorType.Validation);

    public static DomainError ReservationStartTimeInPast(
        DateOnly reservationDate,
        TimeOnly currentTime,
        TimeOnly startTime) => new(
        "ReservationTime.StartTimeInPast",
        $"You are trying to schedule a reservation for today {reservationDate}, the current time" +
        $" is {currentTime}, and you are trying to make a reservation for {startTime} today " +
        $"and it is not possible to schedule a reservation to start in the past",
        ErrorType.BadRequest);

    public static DomainError ReservationEndTimeInPast(
        DateOnly reservationDate,
        TimeOnly currentTime,
        TimeOnly endTime) => new(
        "ReservationTime.EndTimeInPast",
        $"You are trying to schedule a reservation for today {reservationDate}, the current time is " +
        $"{currentTime}, and you are trying to schedule the reservation's end time to be {endTime}, " +
        $"it is not possible to schedule a reservation to end in the past",
        ErrorType.BadRequest);
}