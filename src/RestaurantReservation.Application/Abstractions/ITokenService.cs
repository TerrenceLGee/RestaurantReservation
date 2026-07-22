using RestaurantReservation.Application.Features.Auth.Command.Login;
using RestaurantReservation.Domain.Common;
using RestaurantReservation.Domain.Users;

namespace RestaurantReservation.Application.Abstractions;

public interface ITokenService
{
    Task<Result<LoginResponse>> CreateAuthenticationTokensAsync(
        ApplicationUser user,
        IEnumerable<string> roles,
        CancellationToken cancellationToken = default);

    Result<string> CreateRefreshToken();

    Task<Result> RevokeRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
}