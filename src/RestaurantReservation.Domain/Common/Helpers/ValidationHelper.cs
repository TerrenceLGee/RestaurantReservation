using System.Text.RegularExpressions;

using RestaurantReservation.Domain.Reservations.Errors;

namespace RestaurantReservation.Domain.Common.Helpers;

public static partial class ValidationHelper
{
    public static bool IsRestaurantScheduleStartAndEndTimeValid(TimeOnly? startTime, TimeOnly? endTime)
    {
        if (startTime.HasValue && endTime.HasValue)
        {
            return startTime < endTime;
        }

        return true;
    }

    public static Result EmailAddressValidator(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return Result.Failure(EmailAddressErrors.EmailAddressEmpty);
        }

        if (!IsValidEmail(email))
        {
            return Result.Failure(EmailAddressErrors.EmailAddressInvalid);
        }

        return email.Length > 50
            ? Result.Failure(EmailAddressErrors.EmailAddressMaxLengthExceeded)
            : Result.Success();
    }

    public static Result TelephoneNumberValidator(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
        {
            return Result.Failure(TelephoneNumberErrors.TelephoneNumberEmpty);
        }

        if (phoneNumber.Length > 15)
        {
            return Result.Failure(TelephoneNumberErrors.TelephoneNumberMaxLengthExceeded);
        }

        return !IsValidPhoneNumber(phoneNumber)
            ? Result.Failure(TelephoneNumberErrors.TelephoneNumberInvalid)
            : Result.Success();
    }

    public static Result FirstNameValidator(string firstName)
    {
        if (string.IsNullOrWhiteSpace(firstName))
        {
            return Result.Failure(FirstNameErrors.FirstNameEmpty);
        }

        return firstName.Length > 50
            ? Result.Failure(FirstNameErrors.FirstNameMaxLengthExceeded)
            : Result.Success();
    }

    public static Result LastNameValidator(string lastName)
    {
        if (string.IsNullOrWhiteSpace(lastName))
        {
            return Result.Failure(LastNameErrors.LastNameEmpty);
        }

        return lastName.Length > 50
            ? Result.Failure(LastNameErrors.LastNameMaxLengthExceeded)
            : Result.Success();
    }

    public static Result ReservationDateValidator(DateOnly reservationDate)
    {
        return reservationDate < DateOnly.FromDateTime(DateTime.Today)
            ? Result.Failure(ReservationDateErrors.ReservationDateInvalid)
            : Result.Success();
    }

    public static Result ReservationTimeValidator(
        DateOnly reservationDate,
        TimeOnly startTime,
        TimeOnly endTime)
    {
        if (reservationDate != DateOnly.FromDateTime(DateTime.Today))
        {
            return startTime > endTime
                ? Result.Failure(ReservationTimeErrors.ReservationTimeInvalid)
                : Result.Success();
        }

        var currentTime = TimeOnly.FromDateTime(DateTime.Now);

        if (currentTime > startTime)
        {
            return Result.Failure(ReservationTimeErrors.ReservationStartTimeInPast(
                reservationDate,
                currentTime,
                startTime));
        }

        if (currentTime > endTime)
        {
            return Result.Failure(ReservationTimeErrors.ReservationEndTimeInPast(
                reservationDate,
                currentTime,
                endTime));
        }

        return startTime > endTime
            ? Result.Failure(ReservationTimeErrors.ReservationTimeInvalid)
            : Result.Success();
    }

    private static bool IsValidEmail(string emailAddress)
    {
        return EmailRegex().IsMatch(emailAddress);
    }

    private static bool IsValidPhoneNumber(string phoneNumber)
    {
        return TelephoneRegex().IsMatch(phoneNumber);
    }
    
    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex EmailRegex();
    [GeneratedRegex(@"^[\+]?[1-9]?[\d\s\-\(\)\.]{10,15}$")]
    private static partial Regex TelephoneRegex();
}