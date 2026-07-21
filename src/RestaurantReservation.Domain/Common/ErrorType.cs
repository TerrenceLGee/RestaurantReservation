namespace RestaurantReservation.Domain.Common;

public enum ErrorType
{
    BadRequest,
    CapacityExceeded,
    Conflict,
    Forbidden,
    None,
    NotFound,
    NullValue,
    Unauthorized,
    Validation
}