namespace RestaurantReservation.Domain.Common;

public record DomainError(string Code, string Description, ErrorType ErrorType)
{
    public static readonly DomainError None = new(string.Empty, string.Empty, ErrorType.None);
    public static readonly DomainError NullValue = new(
        "Error.NullValue",
        "Null value was provided",
        ErrorType.NullValue);
}