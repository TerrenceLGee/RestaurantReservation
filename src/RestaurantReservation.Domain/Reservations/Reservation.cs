using RestaurantReservation.Domain.Abstractions;
using RestaurantReservation.Domain.Common;
using RestaurantReservation.Domain.Common.Helpers;
using RestaurantReservation.Domain.Reservations.Errors;
using RestaurantReservation.Domain.Reservations.ValueObjects.CustomerValueObjects;
using RestaurantReservation.Domain.Reservations.ValueObjects.ReservationValueObjects;
using RestaurantReservation.Domain.Restaurants;
using RestaurantReservation.Domain.Users;

namespace RestaurantReservation.Domain.Reservations;

public class Reservation : BaseEntity
{
    public Guid RestaurantId { get; init; }
    public Restaurant? Restaurant { get; set; }
    public string? CustomerId { get; init; }
    public ApplicationUser? Customer { get; set; }
    public string RestaurantName { get; init; }
    public CustomerContactInfo CustomerContactInfo { get; private set; } = null!;
    public ReservationInfo ReservationInfo { get; private set; } = null!;
    public ReservationStatus Status { get; private set; }
    public DateTime ReservationCreatedAtUtc { get; private set; }
    public DateTime? ReservationUpdatedAtUtc { get; set; }
    public DateTime? ReservationCanceledAtUtc { get; set; }
    public DateTime? ReservationCompletedAtUtc { get; set; }
    public ICollection<ReservationTable> Tables { get; set; } = [];
    
    private Reservation() {}

    private Reservation(
        Guid id,
        Guid restaurantId,
        string customerId,
        string restaurantName,
        CustomerContactInfo customerInfo,
        ReservationStatus status,
        ReservationInfo reservationInfo) : base(id)
    {
        RestaurantId = restaurantId;
        CustomerId = customerId;
        RestaurantName = restaurantName;
        CustomerContactInfo = customerInfo;
        Status = status;
        ReservationInfo = reservationInfo;
        ReservationCreatedAtUtc = DateTime.UtcNow;
    }

    public static Result<Reservation> MakeReservation(
        Guid restaurantId,
        string restaurantName,
        string customerId,
        string customerFirstName,
        string customerLastName,
        string customerEmail,
        string customerPhone,
        DateOnly reservationDate,
        TimeOnly reservationStartTime,
        TimeOnly reservationEndTime,
        int numberOfGuests)
    {
        var firstResult = CheckCustomerContactInfo(
            customerFirstName,
            customerLastName,
            customerEmail,
            customerPhone);

        if (firstResult.IsFailure) return Result.Failure<Reservation>(firstResult.Errors);

        var secondResult = CheckReservationInfo(
            reservationDate,
            reservationStartTime,
            reservationEndTime);

        if (secondResult.IsFailure) return Result.Failure<Reservation>(secondResult.Errors);

        var customerInfo = new CustomerContactInfo(
            new FirstName(customerFirstName),
            new LastName(customerLastName),
            new EmailAddress(customerEmail),
            new TelephoneNumber(customerPhone));

        var reservationInfo = new ReservationInfo(
            new ReservationDate(reservationDate),
            new ReservationStart(reservationStartTime),
            new ReservationEnd(reservationEndTime),
            new GuestsInParty(numberOfGuests));

        return Result.Success(new Reservation(
            Guid.CreateVersion7(),
            restaurantId,
            customerId,
            restaurantName,
            customerInfo,
            ReservationStatus.Scheduled,
            reservationInfo));
    }

    public Result UpdateCustomerContactInfo(
        string customerFirstName,
        string customerLastName,
        string customerEmail,
        string customerPhone)
    {
        var validationResult = CheckCustomerContactInfo(
            customerFirstName,
            customerLastName,
            customerEmail,
            customerPhone);

        if (validationResult.IsFailure) return validationResult;

        CustomerContactInfo = new CustomerContactInfo(
            new FirstName(customerFirstName),
            new LastName(customerLastName),
            new EmailAddress(customerEmail),
            new TelephoneNumber(customerPhone));

        ReservationUpdatedAtUtc = DateTime.UtcNow;
        return Result.Success();
    }

    public Result UpdateReservationInfo(
        DateOnly reservationDate,
        TimeOnly reservationStartTime,
        TimeOnly reservationEndTime,
        int numberOfGuests)
    {
        var validationResult = CheckReservationInfo(
            reservationDate,
            reservationStartTime,
            reservationEndTime);

        if (validationResult.IsFailure) return validationResult;

        ReservationInfo = new ReservationInfo(
            new ReservationDate(reservationDate),
            new ReservationStart(reservationStartTime),
            new ReservationEnd(reservationEndTime),
            new GuestsInParty(numberOfGuests));

        ReservationUpdatedAtUtc = DateTime.UtcNow;

        return Result.Success();
    }

    public Result CancelReservation()
    {
        if (Status is ReservationStatus.Canceled or ReservationStatus.Completed)
            return Result.Failure(ReservationErrors.ReservationInvalidCancellationStatus(Status));

        ReservationCanceledAtUtc = DateTime.UtcNow;
        ReservationUpdatedAtUtc = DateTime.UtcNow;
        Status = ReservationStatus.Canceled;

        return Result.Success();
    }

    public Result CompleteReservation()
    {
        if (Status is ReservationStatus.Canceled or ReservationStatus.Completed)
            return Result.Failure(ReservationErrors.ReservationInvalidCompletionStatus(Status));

        ReservationCompletedAtUtc = DateTime.UtcNow;
        ReservationUpdatedAtUtc = DateTime.UtcNow;
        Status = ReservationStatus.Completed;

        return Result.Success();
    }

    private static Result CheckCustomerContactInfo(
        string customerFirstName,
        string customerLastName,
        string customerEmail,
        string customerPhone)
    {
        var errors = new List<DomainError>();
        
        var firstNameResult = ValidationHelper.FirstNameValidator(customerFirstName);
        if (firstNameResult.IsFailure) errors.Add(firstNameResult.Error);

        var lastNameResult = ValidationHelper.LastNameValidator(customerLastName);
        if (lastNameResult.IsFailure) errors.Add(lastNameResult.Error);

        var emailResult = ValidationHelper.EmailAddressValidator(customerEmail);
        if (emailResult.IsFailure) errors.Add(emailResult.Error);

        var phoneNumberResult = ValidationHelper.TelephoneNumberValidator(customerPhone);
        if (phoneNumberResult.IsFailure) errors.Add(phoneNumberResult.Error);

        return errors.Count > 0
            ? Result.Failure(errors)
            : Result.Success();
    }

    private static Result CheckReservationInfo(
        DateOnly reservationDate,
        TimeOnly reservationStartTime,
        TimeOnly reservationEndTime)
    {
        var errors = new List<DomainError>();

        var reservationDateResult = ValidationHelper.ReservationDateValidator(reservationDate);
        if (reservationDateResult.IsFailure) errors.Add(reservationDateResult.Error);

        var reservationTimeResult = ValidationHelper.ReservationTimeValidator(
            reservationDate,
            reservationStartTime,
            reservationEndTime);
        if (reservationTimeResult.IsFailure) errors.Add(reservationTimeResult.Error);

        return errors.Count > 0
            ? Result.Failure(errors)
            : Result.Success();
    }
}