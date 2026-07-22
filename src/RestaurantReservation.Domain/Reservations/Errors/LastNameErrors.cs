using RestaurantReservation.Domain.Common;

namespace RestaurantReservation.Domain.Reservations.Errors;

public static class LastNameErrors
{
    public static readonly DomainError LastNameEmpty = new(
        "LastName.Empty",
        "Last name cannot be empty",
        ErrorType.Validation);

    public static readonly DomainError LastNameMaxLengthExceeded = new(
        "LastName.MaxLengthExceeded",
        "Last name cannot exceed 50 characters",
        ErrorType.Validation);
}