namespace RestaurantReservation.Domain.Tables;

public class TableGroupTable
{
    public Guid TableId { get; set; }
    public Table? Table { get; set; }
    public Guid TableGroupId { get; set; }
    public TableGroup? TableGroup { get; set; }
}