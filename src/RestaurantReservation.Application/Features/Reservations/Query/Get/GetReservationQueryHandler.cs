using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

using RestaurantReservation.Application.Abstractions;
using RestaurantReservation.Application.Features.Constants;
using RestaurantReservation.Application.Features.Reservations.Query.Mappings;
using RestaurantReservation.Application.Features.Reservations.Query.Responses;
using RestaurantReservation.Domain.Common;
using RestaurantReservation.Domain.Reservations.Errors;
using RestaurantReservation.Domain.Users.Errors;

namespace RestaurantReservation.Application.Features.Reservations.Query.Get;

public sealed class GetReservationQueryHandler(
    IApplicationDbContext context,
    HybridCache cache,
    ICurrentUser currentUser,
    ILogger<GetReservationQueryHandler> logger) : IRequestHandler<GetReservationQuery, Result<ReservationDetailResponse>>
{
    public async Task<Result<ReservationDetailResponse>> Handle(
        GetReservationQuery query, 
        CancellationToken cancellationToken)
    {
        var customerId = currentUser.UserId;
        var customerEmail = currentUser.Email ?? "email";

        if (string.IsNullOrWhiteSpace(customerId))
        {
            logger.LogWarning("Customer not found, unable to retrieve reservation");
            return Result.Failure<ReservationDetailResponse>(UserErrors.UserNotFound);
        }

        var reservationKey = $"reservation:{query.ReservationId}";

        var reservation = await cache.GetOrCreateAsync(
            reservationKey,
            async ct =>
            {
                logger.LogInformation("Cache miss for key: {Key}. Retrieving from database.", reservationKey);
                var reservationFromDb = await context.Reservations
                    .Include(r => r.Restaurant)
                    .Include(r => r.Tables)
                    .FirstOrDefaultAsync(r => r.CustomerId == customerId && r.Id == query.ReservationId, ct);
                return reservationFromDb?.ToDetailResponse(reservationFromDb.RestaurantName);
            },
            new HybridCacheEntryOptions
            {
                LocalCacheExpiration = TimeSpan.FromMinutes(5),
                Expiration = TimeSpan.FromMinutes(30)
            },
            tags: [Keys.Reservations],
            cancellationToken: cancellationToken);

        if (reservation is null)
        {
            logger.LogWarning("Reservation {Id} not found for customer with Id {Email}",
                query.ReservationId,
                customerEmail);
            return Result.Failure<ReservationDetailResponse>(
                ReservationErrors.ReservationNotFound(customerEmail));
        }

        return Result.Success(reservation);
    }
}