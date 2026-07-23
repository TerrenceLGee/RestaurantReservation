using RestaurantReservation.Domain.Common;

namespace RestaurantReservation.Domain.Users.Errors;

public static class RegistrationErrors
{
    public static DomainError EmailAlreadyInUse(string email) => new(
        "Email.AlreadyInUse",
        $"The email address '{email}' is already registered in the system",
        ErrorType.Conflict);
}