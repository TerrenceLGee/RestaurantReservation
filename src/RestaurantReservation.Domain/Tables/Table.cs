using RestaurantReservation.Domain.Abstractions;
using RestaurantReservation.Domain.Common;
using RestaurantReservation.Domain.Reservations;
using RestaurantReservation.Domain.Restaurants;

namespace RestaurantReservation.Domain.Tables;

public class Table : BaseEntity
{
    public Guid RestaurantId { get; private set; }
    public Restaurant? Restaurant { get; set; }
    public int SeatsAtTable { get; internal set; }
    public ICollection<TableReservation> ScheduledReservations { get; set; } = [];
    public ICollection<TableGroupTable> TableGroups { get; set; } = [];
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

    public Result<TableReservation> ReserveTable(
        int guestsInParty,
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
        
        ScheduledReservations.Add(tableReservation);

        return Result.Success(tableReservation);
    }
}