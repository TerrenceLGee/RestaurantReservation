using RestaurantReservation.Domain.Common;

namespace RestaurantReservation.Domain.Users.Errors;

public static class EmailErrors
{
    public static DomainError UnableToSendEmail(string address) => new(
        "Email.SendingError",
        $"Unable to send email to address: {address}",
        ErrorType.BadRequest);
}