using RestaurantReservation.Domain.Common;

namespace RestaurantReservation.Domain.Users.Errors;

public static class RegistrationErrors
{
    public static readonly DomainError RegistrationInvalid = new(
        "Invalid.Registration",
        "Unable to register user account at this time because invalid registration parameters",
        ErrorType.BadRequest);

    public static DomainError EmailAlreadyInUse(string email) => new(
        "Email.AlreadyInUse",
        $"The email address '{email}' is already registered in the system",
        ErrorType.Conflict);
}