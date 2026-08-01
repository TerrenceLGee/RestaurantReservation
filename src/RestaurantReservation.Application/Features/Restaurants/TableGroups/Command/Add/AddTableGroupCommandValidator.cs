using FluentValidation;

namespace RestaurantReservation.Application.Features.Restaurants.TableGroups.Command.Add;

internal sealed class AddTableGroupCommandValidator : AbstractValidator<AddTableGroupCommand>
{
    public AddTableGroupCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("{PropertyName} cannot be empty")
            .MaximumLength(50)
            .WithMessage("{PropertyName} cannot exceed 50 characters");
    }
}