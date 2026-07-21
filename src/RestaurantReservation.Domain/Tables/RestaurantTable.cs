using RestaurantReservation.Domain.Abstractions;
using RestaurantReservation.Domain.Restaurants;

namespace RestaurantReservation.Domain.Tables;

public class RestaurantTable : BaseEntity
{
    public Guid RestaurantId { get; private set; }
    public Restaurant? Restaurant { get; set; }
    public int SeatsAtTable { get; internal set; }
    public ICollection<TableGroupTable> TableGroups { get; set; } = [];
    
    private RestaurantTable() {}

    private RestaurantTable(
        Guid id,
        Guid restaurantId,
        int seatsAttable) : base(id)
    {
        RestaurantId = restaurantId;
        SeatsAtTable = seatsAttable;
    }

    public static RestaurantTable Create(Guid restaurantId, int seatsAtTable)
    {
        return new RestaurantTable(
            Guid.CreateVersion7(),
            restaurantId, 
            seatsAtTable);
    }

    public void UpdateSeating(int seatsAtTable)
    {
        SeatsAtTable = seatsAtTable;
    }
}