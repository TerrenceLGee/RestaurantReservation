using RestaurantReservation.Domain.Common;

namespace RestaurantReservation.Domain.Users.Errors;

public static class LoginErrors
{
    public static readonly DomainError InvalidCredentials = new(
        "Invalid.Credentials",
        "The entered credentials were invalid",
        ErrorType.Unauthorized);
}