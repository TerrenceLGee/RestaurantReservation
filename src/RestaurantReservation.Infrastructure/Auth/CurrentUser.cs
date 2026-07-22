using System.Security.Claims;

using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.JsonWebTokens;

using RestaurantReservation.Application.Abstractions;

namespace RestaurantReservation.Infrastructure.Auth;

public class CurrentUser(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    public string? UserId => httpContextAccessor.HttpContext?.User.FindFirstValue(JwtRegisteredClaimNames.Sub);
    public string? Email => httpContextAccessor.HttpContext?.User.FindFirstValue(JwtRegisteredClaimNames.Email);
    public string? Name => httpContextAccessor.HttpContext?.User.FindFirstValue(JwtRegisteredClaimNames.Name);
    public bool IsAuthenticated => httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated ?? false;
}