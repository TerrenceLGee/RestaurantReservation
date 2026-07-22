using Microsoft.AspNetCore.Identity;

using RestaurantReservation.Domain.Reservations;

namespace RestaurantReservation.Domain.Users;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateOnly RegistrationDate { get; set; }
    public ICollection<RefreshToken> RefreshTokens { get; set; } = [];
    public ICollection<Reservation> Reservations { get; set; } = [];
}