using RestaurantReservation.Domain.Common;

namespace RestaurantReservation.Domain.Users.Errors;

public static class UserErrors
{
    public static readonly DomainError UserNotFound = new(
        "User.NotFound",
        "User not found in the system",
        ErrorType.NotFound);
}