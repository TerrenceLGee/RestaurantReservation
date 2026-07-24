using Microsoft.EntityFrameworkCore;

using RestaurantReservation.Domain.Reservations;
using RestaurantReservation.Domain.Restaurants;
using RestaurantReservation.Domain.Tables;
using RestaurantReservation.Domain.Users;

namespace RestaurantReservation.Application.Abstractions;

public interface IApplicationDbContext
{
    DbSet<Restaurant> Restaurants { get; }
    DbSet<Table> Tables { get; }
    DbSet<TableGroup> TableGroups { get; }
    DbSet<Reservation> Reservations { get; }
    DbSet<RefreshToken> RefreshTokens { get; }
    DbSet<ReservationTable> ReservationTables { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}