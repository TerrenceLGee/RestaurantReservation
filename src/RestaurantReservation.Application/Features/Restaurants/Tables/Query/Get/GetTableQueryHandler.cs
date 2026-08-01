using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

using RestaurantReservation.Application.Abstractions;
using RestaurantReservation.Application.Features.Constants;
using RestaurantReservation.Application.Features.Restaurants.Tables.Query.Mappings;
using RestaurantReservation.Application.Features.Restaurants.Tables.Query.Responses;
using RestaurantReservation.Domain.Common;
using RestaurantReservation.Domain.Tables.Errors;

namespace RestaurantReservation.Application.Features.Restaurants.Tables.Query.Get;

public sealed class GetTableQueryHandler(
    IApplicationDbContext context,
    HybridCache cache,
    ILogger<GetTableQueryHandler> logger) : IRequestHandler<GetTableQuery, Result<TableDetailResponse>>
{
    public async Task<Result<TableDetailResponse>> Handle(
        GetTableQuery query, 
        CancellationToken cancellationToken)
    {
        var cacheKey = $"restaurant{query.RestaurantId}_table:{query.TableId}";

        var table = await cache.GetOrCreateAsync(
            cacheKey,
            async ct =>
            {
                logger.LogInformation("Cache miss for key: {Key}. Retrieving from database.", cacheKey);
                var tableFromDb = await context.Tables
                    .AsNoTracking()
                    .Include(t => t.Restaurant)
                    .FirstOrDefaultAsync(t => t.Id == query.TableId && t.RestaurantId == query.RestaurantId,
                        ct);
                return tableFromDb?.ToDetailResponse();
            },
            new HybridCacheEntryOptions
            {
                LocalCacheExpiration = TimeSpan.FromMinutes(4),
                Expiration = TimeSpan.FromMinutes(30)
            },
            tags: [Keys.Tables],
            cancellationToken: cancellationToken).ConfigureAwait(false);

        return table is null
            ? Result.Failure<TableDetailResponse>(TableErrors.TableNotFound)
            : Result.Success(table);
    }
}