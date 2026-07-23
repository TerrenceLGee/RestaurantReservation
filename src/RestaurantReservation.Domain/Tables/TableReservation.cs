namespace RestaurantReservation.Domain.Tables;

public record TableReservation(DateOnly ReservationDay, TimeOnly ReservationStart, TimeOnly ReservationEnd)
{
    public Guid? ReservationId { get; set; }
}