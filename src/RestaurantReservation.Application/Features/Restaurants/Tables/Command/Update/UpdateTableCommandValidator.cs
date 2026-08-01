using FluentValidation;

namespace RestaurantReservation.Application.Features.Restaurants.Tables.Command.Update;

internal sealed class UpdateTableCommandValidator : AbstractValidator<UpdateTableCommand>
{
    public UpdateTableCommandValidator()
    {
        RuleFor(x => x.NumberOfSeats)
            .GreaterThan(0)
            .WithMessage("{PropertyName} must be greater than 0");

        RuleFor(x => x.GroupName)
            .MaximumLength(50)
            .WithMessage("{PropertyName} cannot exceed 50 characters");
    }
}