using RestaurantReservation.Domain.Abstractions;
using RestaurantReservation.Domain.Common;
using RestaurantReservation.Domain.Reservations;
using RestaurantReservation.Domain.Reservations.ValueObjects.ReservationValueObjects;
using RestaurantReservation.Domain.Restaurants;

namespace RestaurantReservation.Domain.Tables;

public class Table : BaseEntity
{
    public Guid RestaurantId { get; private set; }
    public Restaurant? Restaurant { get; set; }
    public int SeatsAtTable { get; internal set; }
    public ICollection<TableGroup> TableGroups { get; set; } = [];
    public ICollection<ReservationTable> Reservations {get; set;} = [];
    
    private Table() {}

    private Table(
        Guid id,
        Guid restaurantId,
        int seatsAtTable) : base(id)
    {
        RestaurantId = restaurantId;
        SeatsAtTable = seatsAtTable;
    }

    public static Table Create(Guid restaurantId, int seatsAtTable)
    {
        return new Table(
            Guid.CreateVersion7(),
            restaurantId, 
            seatsAtTable);
    }

    public void UpdateSeating(int seatsAtTable)
    {
        SeatsAtTable = seatsAtTable;
    }

    public Result<ReservationTable> ReserveTable(
        DateOnly reservationDate,
        TimeOnly startOfParty,
        TimeOnly endOfParty,
        Guid? reservationId = null)
    {
        var tableReservation = new TableReservation(
            reservationDate,
            startOfParty,
            endOfParty);

        if (reservationId.HasValue)
        {
            tableReservation.ReservationId = reservationId.Value;
        }

        var reservationTable = new ReservationTable { TableId = Id, ScheduledReservation = tableReservation };
        
        Reservations.Add(reservationTable);

        return Result.Success(reservationTable);
    }

    public Result<ReservationTable> UpdateTableReservation(
        Guid reservationId,
        DateOnly reservationDate,
        TimeOnly reservationStartTime,
        TimeOnly reservationEndTime)
    {
        throw new InvalidOperationException();
    }
}