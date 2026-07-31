using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

using RestaurantReservation.Application.Abstractions;
using RestaurantReservation.Application.Features.Constants;
using RestaurantReservation.Application.Features.Restaurants.TableGroups.Query.Mappings;
using RestaurantReservation.Application.Features.Restaurants.TableGroups.Query.Responses;
using RestaurantReservation.Domain.Common;
using RestaurantReservation.Domain.Tables.Errors;

namespace RestaurantReservation.Application.Features.Restaurants.TableGroups.Query.Get;

public sealed class GetTableGroupQueryHandler(
    IApplicationDbContext context,
    HybridCache cache,
    ILogger<GetTableGroupQueryHandler> logger) : IRequestHandler<GetTableGroupQuery, Result<TableGroupDetailResponse>>
{
    public async Task<Result<TableGroupDetailResponse>> Handle(
        GetTableGroupQuery query, 
        CancellationToken cancellationToken)
    {
        var cacheKey = $"restaurant:{query.RestaurantId}_tableGroup:{query.TableGroupId}";

        var tableGroup = await cache.GetOrCreateAsync(
            cacheKey,
            async ct =>
            {
                logger.LogInformation("Cache miss for key: {Key}. Retrieving from database.", cacheKey);
                var tableGroupFromDb = await context.TableGroups
                    .AsNoTracking()
                    .Include(tg => tg.Restaurant)
                    .Include(tg => tg.Tables)
                    .FirstOrDefaultAsync(tg => tg.Id == query.TableGroupId && tg.RestaurantId == query.RestaurantId,
                        ct);
                return tableGroupFromDb?.ToDetailResponse();
            }, 
            new HybridCacheEntryOptions
            {
                LocalCacheExpiration = TimeSpan.FromMinutes(4),
                Expiration = TimeSpan.FromMinutes(30)
            },
            tags: [Keys.TableGroups],
            cancellationToken: cancellationToken);

        return tableGroup is null
            ? Result.Failure<TableGroupDetailResponse>(TableGroupErrors.TableGroupNotFound())
            : Result.Success(tableGroup);
    }
}