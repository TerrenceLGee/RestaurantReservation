using RestaurantReservation.Domain.Common;

namespace RestaurantReservation.Domain.Users.Errors;

public static class RefreshTokenErrors
{
    public static readonly DomainError RefreshTokenInvalid = new(
        "RefreshToken.Invalid",
        "Unable to revoke refresh token because it is invalid",
        ErrorType.Forbidden);
}