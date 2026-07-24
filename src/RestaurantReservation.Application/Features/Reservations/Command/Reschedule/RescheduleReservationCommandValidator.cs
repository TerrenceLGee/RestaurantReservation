using FluentValidation;

namespace RestaurantReservation.Application.Features.Reservations.Command.Reschedule;

public class RescheduleReservationCommandValidator : AbstractValidator<RescheduleReservationCommand>
{
    public RescheduleReservationCommandValidator()
    {
        RuleFor(x => x.RestaurantName)
            .NotEmpty()
            .WithMessage("{PropertyName} cannot be empty")
            .MaximumLength(50)
            .WithMessage("{PropertyName} cannot exceed 50 characters");

        RuleFor(x => x.RescheduleDate)
            .GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today))
            .WithMessage("{PropertyName} cannot be in the past");

        RuleFor(x => x.RescheduleStartTime)
            .LessThan(x => x.RescheduleEndTime)
            .WithMessage("{PropertyName} must come after the reservation start time");

        RuleFor(x => x.RescheduleEndTime)
            .GreaterThan(x => x.RescheduleStartTime)
            .WithMessage("{PropertyName} must come after the reservation start time");

        RuleFor(x => x.RescheduleNumberOfGuests)
            .GreaterThan(0)
            .WithMessage(
                "{PropertyName} must be greater than zero. Even if you are the only person in your party that still counts as one guest");
    }
}