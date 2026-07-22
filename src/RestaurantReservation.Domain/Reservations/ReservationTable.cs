using RestaurantReservation.Domain.Tables;

namespace RestaurantReservation.Domain.Reservations;

public class ReservationTable
{
    private Guid ReservationId { get; set; }
    public Reservation? Reservation { get; set; }
    private Guid TableId { get; set; }
    public RestaurantTable? Table { get; set; }
}