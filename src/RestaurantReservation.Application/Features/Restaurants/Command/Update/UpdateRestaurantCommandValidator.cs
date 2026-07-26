using FluentValidation;

using RestaurantReservation.Domain.Restaurants;

namespace RestaurantReservation.Application.Features.Restaurants.Command.Update;

public sealed class UpdateRestaurantCommandValidator : AbstractValidator<UpdateRestaurantCommand>
{
    public UpdateRestaurantCommandValidator()
    {
        RuleFor(x => x.Name)
            .MaximumLength(50)
            .WithMessage("{PropertyName} cannot exceed 50 characters.");

        RuleFor(x => x.Schedule)
            .Must(CheckForValidSchedule)
            .WithMessage("{PropertyName} must have each day's opening begin before the day's closing hours.");
    }

    private static bool CheckForValidSchedule(RestaurantSchedule[]? schedule)
    {
        if (schedule is not null)
        {
            if (schedule.Length != 7) return false;
            foreach (var day in schedule)
            {
                if (day.DailyHours[0] > day.DailyHours[1]) return false;
            }
        }

        return true;
    }
}