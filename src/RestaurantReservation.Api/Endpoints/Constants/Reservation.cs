namespace RestaurantReservation.Api.Endpoints.Constants;

public static class Reservation
{
    public static string BaseUri => "/api/reservations";
    public static string Tag => "Reservations";
    public static string ScheduleReservation => "/schedulereservation";
    public static string RescheduleReservation => "/reschedulereservation";
    public static string CancelReservation => "/cancelreservation";
    public static string CompleteReservation => "/completereservation";
    public static string GetAll => "/";
    public static string Get => "/{reservationId:guid}";
}