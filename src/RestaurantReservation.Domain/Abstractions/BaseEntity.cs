namespace RestaurantReservation.Domain.Abstractions;

public abstract class BaseEntity
{
    public Guid Id { get; init; }

    protected BaseEntity(Guid id)
    {
        Id = id;
    }
    
    protected BaseEntity() {}
}