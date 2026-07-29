using MediatR;

using Microsoft.Extensions.Logging;

using RestaurantReservation.Application.Abstractions;
using RestaurantReservation.Domain.Common;

namespace RestaurantReservation.Application.Features.Auth.Command.Logout;

public sealed class LogoutCommandHandler(
    ITokenService tokenService,
    ICurrentUser currentUser,
    ILogger<LogoutCommandHandler> logger) : IRequestHandler<LogoutCommand, Result>
{
    public async Task<Result> Handle(
        LogoutCommand command, 
        CancellationToken cancellationToken)
    {
        var currentUserId = currentUser.UserId;
        var currentUserEmail = currentUser.Email;
        var currentUserName = currentUser.Name;

        var result = await tokenService.RevokeRefreshTokenAsync(command.RefreshToken, cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning("{Name} ({Email}) with id {Id} attempted to logout with a revoked or fake refresh token",
                currentUserName,
                currentUserEmail,
                currentUserId);
            return result;
        }
        
        logger.LogInformation("{Name} ({Email}) has logged out of the system at {Utc}",
            currentUserName,
            currentUserEmail,
            DateTime.UtcNow);
        return Result.Success();
    }
}