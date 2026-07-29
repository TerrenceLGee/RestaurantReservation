using MediatR;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

using RestaurantReservation.Domain.Common;
using RestaurantReservation.Domain.Users;
using RestaurantReservation.Domain.Users.Errors;

namespace RestaurantReservation.Application.Features.Auth.Command.Register;

public sealed class RegisterUserCommandHandler(
    UserManager<ApplicationUser> userManager,
    ILogger<RegisterUserCommandHandler> logger) : IRequestHandler<RegisterUserCommand, Result>
{
    public async Task<Result> Handle(
        RegisterUserCommand command, 
        CancellationToken cancellationToken)
    {
        var existingUser = await userManager
            .FindByEmailAsync(command.EmailAddress);

        if (existingUser is not null)
        {
            logger.LogWarning("An attempt was made to register a new account with email: {Email}. This email is already registered with an account in the system",
                command.EmailAddress);
            return Result.Failure(RegistrationErrors.EmailAlreadyInUse(command.EmailAddress));
        }

        var userToAdd = new ApplicationUser
        {
            FirstName = command.FirstName,
            LastName = command.LastName,
            PhoneNumber = command.PhoneNumber,
            Email = command.EmailAddress,
            UserName = command.EmailAddress,
            RegistrationDate = new DateOnly(
                DateTime.Now.Year,
                DateTime.Now.Month,
                DateTime.Now.Day)
        };

        var userAddedResult = await userManager.CreateAsync(userToAdd, command.Password);

        if (!userAddedResult.Succeeded)
        {
            var errors = userAddedResult.Errors.Select(e => new DomainError(
                    e.Code,
                    e.Description,
                    ErrorType.BadRequest))
                .ToList();
            logger.LogError("Unable to add user {Email}: {FirstError}",
                command.EmailAddress,
                errors[0].Description);

            return Result.Failure(errors);
        }

        var userAddedAsCustomerResult = await userManager.AddToRoleAsync(userToAdd, Roles.Customer);

        if (!userAddedAsCustomerResult.Succeeded)
        {
            var errors = userAddedAsCustomerResult.Errors.Select(e => new DomainError(
                    e.Code,
                    e.Description,
                    ErrorType.BadRequest))
                .ToList();
            logger.LogError("Unable to add user {Email} to the {Role} role: {FirstError}",
                command.EmailAddress,
                Roles.Customer,
                errors[0].Description);

            return Result.Failure(errors);
        }

        return Result.Success();
    }
}