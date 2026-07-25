using FluentValidation;

namespace RestaurantReservation.Application.Features.Reservations.Command.Cancel;

public sealed class CancelReservationCommandValidator : AbstractValidator<CancelReservationCommand>
{
    public CancelReservationCommandValidator()
    {
        RuleFor(x => x.RestaurantName)
            .NotEmpty()
            .WithMessage("{PropertyName} cannot be empty")
            .MaximumLength(50)
            .WithMessage("{PropertyName} cannot exceed 50 characters");

        RuleFor(x => x.ReservationDate)
            .GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today))
            .WithMessage("{PropertyName} cannot be in the past");

        RuleFor(x => x.ReservationStartTime)
            .LessThan(x => x.ReservationEndTime)
            .WithMessage("{PropertyName} must come before the reservation end time");

        RuleFor(x => x.ReservationEndTime)
            .GreaterThan(x => x.ReservationStartTime)
            .WithMessage("{PropertyName} must come after the reservation end time");
    }
}