using MediatR;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

using RestaurantReservation.Application.Abstractions;
using RestaurantReservation.Domain.Common;
using RestaurantReservation.Domain.Users;
using RestaurantReservation.Domain.Users.Errors;

namespace RestaurantReservation.Application.Features.Auth.Command.Login;

public sealed class LoginCommandHandler(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    ILogger<LoginCommandHandler> logger,
    ITokenService tokenService) : IRequestHandler<LoginCommand, Result<LoginResponse>>
{
    public async Task<Result<LoginResponse>> Handle(
        LoginCommand command, 
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(command.Email);

        if (user is null)
        {
            logger.LogWarning("User {Email} not found", command.Email);
            return Result.Failure<LoginResponse>(UserErrors.UserNotFound);
        }

        var checkPassword = await signInManager.CheckPasswordSignInAsync(
            user,
            command.Password,
            false);

        if (!checkPassword.Succeeded)
        {
            logger.LogWarning("User {Email} attempted to login with invalid credentials", command.Email);
            return Result.Failure<LoginResponse>(LoginErrors.InvalidCredentials);
        }

        var roles = await userManager.GetRolesAsync(user);

        var response = await tokenService.CreateAuthenticationTokensAsync(
            user, 
            roles, 
            cancellationToken);
        
        logger.LogInformation("({Email}) has logged into the system at {Utc}",
            command.Email,
            DateTime.UtcNow);

        return Result.Success(response.Value);
    }
}