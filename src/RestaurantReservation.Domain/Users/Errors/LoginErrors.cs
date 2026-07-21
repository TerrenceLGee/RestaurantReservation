using RestaurantReservation.Domain.Common;

namespace RestaurantReservation.Domain.Users.Errors;

public static class LoginErrors
{
    public static readonly DomainError InvalidCredentials = new(
        "Invalid.Credentials",
        "the entered credentials were invalid",
        ErrorType.Unauthorized);
}