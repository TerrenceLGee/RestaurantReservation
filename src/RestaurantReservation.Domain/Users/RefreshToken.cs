using RestaurantReservation.Domain.Abstractions;

namespace RestaurantReservation.Domain.Users;

public class RefreshToken : BaseEntity
{
    public string? UserId { get; set; }
    public ApplicationUser? User { get; set; }
    public string? Token { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }
    public DateTime? RevokedAt { get; set; }
    public bool IsRevoked { get; set; }
    public bool IsActive => RevokedAt is null && DateTime.UtcNow < RefreshTokenExpiryTime;
}