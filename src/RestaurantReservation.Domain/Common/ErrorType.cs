namespace RestaurantReservation.Domain.Common;

public enum ErrorType
{
    BadRequest,
    CapacityExceeded,
    Conflict,
    Forbidden,
    InternalServerError,
    None,
    NotFound,
    NullValue,
    Unauthorized,
    Validation
}