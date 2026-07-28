using FluentValidation;

using RestaurantReservation.Domain.Restaurants;

namespace RestaurantReservation.Application.Features.Restaurants.Command.Add;

internal sealed class AddRestaurantCommandValidator : AbstractValidator<AddRestaurantCommand>
{
    public AddRestaurantCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("{PropertyName} cannot be empty")
            .MaximumLength(50)
            .WithMessage("{PropertyName} cannot exceed 50 characters.");
        
        RuleFor(x => x.Schedule)
            .Must(CheckForValidSchedule)
            .WithMessage("{PropertyName] must have each day's opening hours begin before the days closing hours.");

        RuleFor(x => x.TableInfo)
            .Must(CheckForValidTableInfo)
            .WithMessage("{PropertyName} is invalid, please review your input and try again");
    }

    private static bool CheckForValidSchedule(RestaurantSchedule[] schedule)
    {
        if (schedule.Length != 7) return false;
        
        foreach (var day in schedule)
        {
            if (day.DailyHours[0] > day.DailyHours[1]) return false;
        }

        return true;
    }

    private static bool CheckForValidTableInfo(TableInfo[] tableInfo)
    {
        foreach (var table in tableInfo)
        {
            if (table.NumberOfSeats <= 0) return false;
            if (table.NumberOfTables <= 0) return false;
            if (!string.IsNullOrEmpty(table.GroupName))
            {
                if (table.GroupName.Length > 50) return false;
            }
        }

        return true;
    }
}