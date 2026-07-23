using RestaurantReservation.Domain.Abstractions;
using RestaurantReservation.Domain.Restaurants;

namespace RestaurantReservation.Domain.Tables;

public class TableGroup : BaseEntity
{
    public string Name { get; private set; }
    public Guid RestaurantId { get; private set; }
    public Restaurant? Restaurant { get; set; }
    public ICollection<TableGroupTable> Tables { get; private set; } = [];
    
    private TableGroup() {}

    private TableGroup(Guid id, Guid restaurantId, string name) : base(id)
    {
        RestaurantId = restaurantId;
        Name = name;
    }

    public static TableGroup Create(Guid restaurantId, string name)
    {
        return new TableGroup(
            Guid.CreateVersion7(),
            restaurantId,
            name);
    }

    public void AddTables(List<Table> tables)
    {
        foreach (var table in tables)
        {
            if (table.RestaurantId != RestaurantId)
            {
                throw new InvalidOperationException(
                    "You cannot add a table to a group unless they are in the same restaurant");
            }

            if (Tables.All(t => t.TableId != table.Id))
            {
                Tables.Add(new TableGroupTable
                {
                    TableId = table.Id,
                    TableGroupId = Id
                });
            }
        }
    }
}