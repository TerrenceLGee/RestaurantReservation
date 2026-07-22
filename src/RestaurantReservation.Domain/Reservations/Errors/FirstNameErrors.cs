using RestaurantReservation.Domain.Common;

namespace RestaurantReservation.Domain.Reservations.Errors;

public static class FirstNameErrors
{
    public static readonly DomainError FirstNameEmpty = new(
        "FirstName.Empty",
        "First name cannot be empty",
        ErrorType.Validation);

    public static readonly DomainError FirstNameMaxLengthExceeded = new(
        "FirstName.MaxLengthExceeded",
        "First name cannot exceed 50 characters",
        ErrorType.Validation);
}