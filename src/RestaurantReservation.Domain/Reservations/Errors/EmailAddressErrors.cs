using RestaurantReservation.Domain.Common;

namespace RestaurantReservation.Domain.Reservations.Errors;

public static class EmailAddressErrors
{
    public static readonly DomainError EmailAddressInvalid = new(
        "EmailAddress.Invalid",
        "Email address is not in a valid format",
        ErrorType.Validation);

    public static readonly DomainError EmailAddressEmpty = new(
        "EmailAddress.Empty",
        "Email address cannot be empty",
        ErrorType.Validation);

    public static readonly DomainError EmailAddressMaxLengthExceeded = new(
        "EmailAddress.NaxLengthExceeded",
        "Email address cannot exceed 50 characters",
        ErrorType.Validation);
}