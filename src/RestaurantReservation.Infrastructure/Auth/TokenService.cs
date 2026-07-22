using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

using RestaurantReservation.Application.Abstractions;
using RestaurantReservation.Application.Features.Auth.Command.Login;
using RestaurantReservation.Domain.Common;
using RestaurantReservation.Domain.Users;
using RestaurantReservation.Domain.Users.Errors;

namespace RestaurantReservation.Infrastructure.Auth;

public class TokenService(
    IOptions<JwtSettings> jwtSettings,
    IApplicationDbContext context) : ITokenService
{
    private readonly JwtSettings _settings = jwtSettings.Value;

    public async Task<Result<LoginResponse>> CreateAuthenticationTokensAsync(
        ApplicationUser user, 
        IEnumerable<string> roles,
        CancellationToken cancellationToken = default)
    {
        var accessTokenExpiresAt = DateTime.UtcNow.AddMinutes(_settings.ExpiryInMinutes);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.Email, user.Email!),
            new(JwtRegisteredClaimNames.Name, $"{user.FirstName} {user.LastName}"),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Iss, _settings.Issuer),
            new(JwtRegisteredClaimNames.Aud, _settings.Audience)
        };
        
        claims.AddRange(roles.Select(role => new Claim("role", role)));

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Key));
        var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var descriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = accessTokenExpiresAt,
            Issuer = _settings.Issuer,
            Audience = _settings.Audience,
            SigningCredentials = credentials
        };

        var handler = new JsonWebTokenHandler();
        var accessToken = handler.CreateToken(descriptor);
        var refreshToken = CreateRefreshToken();
        var refreshTokenExpiryTime = DateTime.UtcNow.AddDays(_settings.RefreshTokenExpirationInDays);

        var newRefreshToken = new RefreshToken
        {
            UserId = user.Id,
            Token = refreshToken.Value,
            RefreshTokenExpiryTime = refreshTokenExpiryTime,
            IsRevoked = false
        };

        await context.RefreshTokens.AddAsync(newRefreshToken, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success(
            new LoginResponse(
                accessToken,
                refreshToken.Value,
                accessTokenExpiresAt,
                refreshTokenExpiryTime));
    }

    public Result<string> CreateRefreshToken()
    {
        var randomBytes = RandomNumberGenerator.GetBytes(64);
        return Result.Success(Convert.ToBase64String(randomBytes));
    }

    public async Task<Result> RevokeRefreshTokenAsync(
        string refreshToken, 
        CancellationToken cancellationToken = default)
    {
        var tokenToRevoke = await context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken, cancellationToken);

        if (tokenToRevoke is null || !tokenToRevoke.IsActive)
        {
            return Result.Failure(RefreshTokenErrors.RefreshTokenInvalid);
        }

        tokenToRevoke.IsRevoked = true;
        tokenToRevoke.RevokedAt = DateTime.UtcNow;
        await context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}