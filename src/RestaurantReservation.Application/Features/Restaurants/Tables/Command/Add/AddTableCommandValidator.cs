using FluentValidation;

namespace RestaurantReservation.Application.Features.Restaurants.Tables.Command.Add;

internal sealed class AddTableCommandValidator : AbstractValidator<AddTableCommand>
{
    public AddTableCommandValidator()
    {
        RuleFor(x => x.NumberOfSeats)
            .NotEmpty()
            .GreaterThan(0)
            .WithMessage("{PropertyName} must be greater than 0");

        RuleFor(x => x.TableGroup)
            .MaximumLength(50)
            .WithMessage("{PropertyName} cannot be greater than 50 characters");
    }
}