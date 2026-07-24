using System.Text.RegularExpressions;

using FluentValidation;

namespace RestaurantReservation.Application.Features.Auth.Command.Register;

public partial class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage("{PropertyName} cannot be empty")
            .MaximumLength(50)
            .WithMessage("{PropertyName} cannot exceed 50 characters");

        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage("{PropertyName} cannot be empty")
            .MaximumLength(50)
            .WithMessage("{PropertyName} cannot exceed 50 characters");

        RuleFor(x => x.EmailAddress)
            .NotEmpty()
            .WithMessage("{PropertyName} cannot be empty")
            .MaximumLength(50)
            .EmailAddress()
            .WithMessage("{PropertyName} is not in a valid format");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .WithMessage("{PropertyName} cannot be empty")
            .Must(IsValidTelephoneNumber)
            .WithMessage("{PropertyName} is not in a valid telephone number format");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("{PropertyName} cannot be empty")
            .MinimumLength(8)
            .Must(IsValidPassword)
            .WithMessage("{PropertyName} is not a valid password");

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty()
            .WithMessage("{PropertyName} cannot be empty")
            .Equal(x => x.Password)
            .WithMessage("Passwords do not match");
    }
    
    private static bool IsValidTelephoneNumber(string phoneNumber)
    {
        return PhoneNumberRegex().IsMatch(phoneNumber);
    }

    private static bool IsValidPassword(string password)
    {
        var hasUpper = false;
        var hasLower = false;
        var hasNumeric = false;

        foreach (var letter in password)
        {
            if (Char.IsUpper(letter)) hasUpper = true;
            if (Char.IsLower(letter)) hasLower = true;
            if (Char.IsNumber(letter)) hasNumeric = true;
        }

        return hasUpper && hasLower && hasNumeric;
    }

    [GeneratedRegex(@"^[\+]?[1-9]?[\d\s\-\(\)\.]{10,15}$")]
    private static partial Regex PhoneNumberRegex();
}