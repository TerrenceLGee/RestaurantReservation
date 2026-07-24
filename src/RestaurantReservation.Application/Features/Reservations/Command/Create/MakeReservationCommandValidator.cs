using System.Text.RegularExpressions;

using FluentValidation;

namespace RestaurantReservation.Application.Features.Reservations.Command.Create;

public sealed partial class MakeReservationCommandValidator : AbstractValidator<MakeReservationCommand>
{
    public MakeReservationCommandValidator()
    {
        RuleFor(x => x.CustomerFirstName)
            .NotEmpty()
            .WithMessage("{PropertyName} cannot be empty")
            .MaximumLength(50)
            .WithMessage("{PropertyName} cannot exceed 50 characters");

        RuleFor(x => x.CustomerLastName)
            .NotEmpty()
            .WithMessage("{PropertyName} cannot be empty")
            .MaximumLength(50)
            .WithMessage("{PropertyName} cannot exceed 50 characters");

        RuleFor(x => x.CustomerEmail)
            .NotEmpty()
            .WithMessage("{PropertyName} cannot be empty")
            .MaximumLength(50)
            .WithMessage("{PropertyName} cannot exceed 50 characters")
            .EmailAddress()
            .WithMessage("{PropertyName} is not a valid email address");

        RuleFor(x => x.CustomerPhone)
            .NotEmpty()
            .WithMessage("{PropertyName} cannot be empty")
            .Must(IsValidTelephoneNumber)
            .WithMessage("{PropertyName} is not a valid telephone number");

        RuleFor(x => x.ReservationDate)
            .GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today))
            .WithMessage("{PropertyName} cannot be in the past");

        RuleFor(x => x.ReservationStartTime)
            .LessThan(x => x.ReservationEndTime)
            .WithMessage("{PropertyName} must come after the reservation start time");

        RuleFor(x => x.ReservationEndTime)
            .GreaterThan(x => x.ReservationStartTime)
            .WithMessage("{PropertyName} must come after the reservation start time");

        RuleFor(x => x.NumberOfGuests)
            .GreaterThan(0)
            .WithMessage("{PropertyName} must be greater than zero, even if you are the only person in your party " +
                         "that still counts as one guest");
    }
    
    private static bool IsValidTelephoneNumber(string phoneNumber)
    {
        return PhoneNumberRegex().IsMatch(phoneNumber);
    }

    [GeneratedRegex(@"^[\+]?[1-9]?[\d\s\-\(\)\.]{10,15}$")]
    private static partial Regex PhoneNumberRegex();
}