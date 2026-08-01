using FluentValidation;

namespace RestaurantReservation.Application.Features.Restaurants.TableGroups.Command.Update;

internal sealed class UpdateTableGroupCommandValidator : AbstractValidator<UpdateTableGroupCommand>
{
    public UpdateTableGroupCommandValidator()
    {
        RuleFor(x => x.GroupName)
            .MaximumLength(50)
            .WithMessage("{PropertyName} cannot exceed 50 characters");
    }
}
