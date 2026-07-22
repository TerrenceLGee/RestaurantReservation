using RestaurantReservation.Domain.Common;

namespace RestaurantReservation.Domain.Reservations.Errors;

public static class TelephoneNumberErrors
{
    public static readonly DomainError TelephoneNumberInvalid = new(
        "PhoneNumber.Invalid",
        "The telephone number is not in a valid format",
        ErrorType.Validation);

    public static readonly DomainError TelephoneNumberEmpty = new(
        "PhoneNumber.Empty",
        "The telephone number cannot be empty",
        ErrorType.Validation);

    public static readonly DomainError TelephoneNumberMaxLengthExceeded = new(
        "PhoneNumber.MaxLengthExceeded",
        "The telephone number cannot exceed 15 characters",
        ErrorType.Validation);
}