namespace RestaurantReservation.Domain.Reservations.ValueObjects.ReservationValueObjects;

public record TableReservation(DateOnly ReservationDay, TimeOnly ReservationStart, TimeOnly ReservationEnd)
{
    public Guid? ReservationId { get; set; }
}