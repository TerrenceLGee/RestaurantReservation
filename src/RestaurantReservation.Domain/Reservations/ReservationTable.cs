using RestaurantReservation.Domain.Reservations.ValueObjects;
using RestaurantReservation.Domain.Tables;

namespace RestaurantReservation.Domain.Reservations;

public class ReservationTable
{
    public Guid ReservationId { get; set; }
    public Reservation? Reservation { get; set; }
    public Guid TableId { get; set; }
    public Table? Table { get; set; }
}