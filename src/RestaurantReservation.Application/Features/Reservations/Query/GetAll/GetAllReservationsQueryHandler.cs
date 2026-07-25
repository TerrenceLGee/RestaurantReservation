using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

using RestaurantReservation.Application.Abstractions;
using RestaurantReservation.Application.Extensions;
using RestaurantReservation.Application.Features.Constants;
using RestaurantReservation.Application.Features.Reservations.Query.Mappings;
using RestaurantReservation.Application.Features.Reservations.Query.Responses;
using RestaurantReservation.Domain.Common;
using RestaurantReservation.Domain.Users.Errors;

namespace RestaurantReservation.Application.Features.Reservations.Query.GetAll;

public sealed class GetAllReservationsQueryHandler(
    IApplicationDbContext context,
    HybridCache cache,
    ICurrentUser currentUser,
    ILogger<GetAllReservationsQueryHandler> logger) : IRequestHandler<GetAllReservationsQuery, Result<PagedResult<ReservationResponse>>>
{
    public async Task<Result<PagedResult<ReservationResponse>>> Handle(
        GetAllReservationsQuery query, 
        CancellationToken cancellationToken)
    {
        var customerId = currentUser.UserId;

        if (string.IsNullOrWhiteSpace(customerId))
        {
            logger.LogWarning("User not found in the system, unable to retrieve reservations");
            return Result.Failure<PagedResult<ReservationResponse>>(UserErrors.UserNotFound);
        }

        var reservationsKey = $"reservations_page={query.Page}_pageSize={query.PageSize}";

        (List<ReservationResponse> items, int totalCount) = await cache.GetOrCreateAsync(
            reservationsKey,
            async ct =>
            {
                logger.LogInformation("Cache miss for key: {Key}. Retrieving from database",
                    reservationsKey);

                var reservationsQuery = context.Reservations
                    .Where(r => r.CustomerId == customerId)
                    .AsNoTracking()
                    .AsQueryable();

                if (!string.IsNullOrWhiteSpace(query.RestaurantName))
                {
                    reservationsQuery = reservationsQuery.ApplySearch(query.RestaurantName);
                }

                if (!string.IsNullOrWhiteSpace(query.Status))
                {
                    reservationsQuery = reservationsQuery.ApplySearch(query.Status);
                }

                var totalCount = await reservationsQuery
                    .AsNoTracking()
                    .CountAsync(r => r.CustomerId != null
                                     && r.CustomerId.Equals(customerId), ct);

                reservationsQuery = reservationsQuery.ApplySort(
                    string.IsNullOrWhiteSpace(query.SortBy) ? "Id" : query.SortBy);

                var items = await reservationsQuery
                    .AsNoTracking()
                    .ApplyPagination(query.Page, query.PageSize)
                    .Select(r => r.ToResponse(r.RestaurantName))
                    .ToListAsync(ct);

                return (items, totalCount);
            },
            new HybridCacheEntryOptions
            {
                LocalCacheExpiration = TimeSpan.FromMinutes(5),
                Expiration = TimeSpan.FromMinutes(30)
            },
            tags: [Keys.Reservations],
            cancellationToken: cancellationToken);

        var pagedResult = new PagedResult<ReservationResponse>(items, totalCount, query.Page, query.PageSize);
        return Result.Success(pagedResult);
    }
}