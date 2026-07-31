using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

using RestaurantReservation.Application.Abstractions;
using RestaurantReservation.Application.Extensions;
using RestaurantReservation.Application.Features.Constants;
using RestaurantReservation.Application.Features.Restaurants.Query.Mappings;
using RestaurantReservation.Application.Features.Restaurants.Query.Response;
using RestaurantReservation.Domain.Common;
using RestaurantReservation.Domain.Restaurants.Errors;

namespace RestaurantReservation.Application.Features.Restaurants.Query.Get;

public sealed class GetRestaurantQueryHandler(
    IApplicationDbContext context,
    HybridCache cache,
    ILogger<GetRestaurantQueryHandler> logger) : IRequestHandler<GetRestaurantQuery, Result<RestaurantDetailResponse>>
{
    public async Task<Result<RestaurantDetailResponse>> Handle(
        GetRestaurantQuery query, 
        CancellationToken cancellationToken)
    {
        var cacheKey = $"restaurant:{query.Id}_tablePage={query.TablePage}_tablePageSize={query.TablePageSize}";

        var restaurant = await cache.GetOrCreateAsync(
            cacheKey,
            async ct =>
            {
                logger.LogInformation("Cache miss for key: {Key}. Retrieving from database.", cacheKey);
                var restaurantFromDb = await context.Restaurants
                    .AsNoTracking()
                    .Include(r => r.TableGroups)
                    .AsSplitQuery()
                    .FirstOrDefaultAsync(r => r.Id == query.Id, ct);

                var totalTableCount = await context.Tables
                    .AsNoTracking()
                    .CountAsync(t => t.RestaurantId == query.Id, ct);

                var restaurantTablesFromDb = await context.Tables
                    .AsNoTracking()
                    .Where(t => t.RestaurantId == query.Id)
                    .ApplySort("TableGroupName asc")
                    .ApplyPagination(query.TablePage, query.TablePageSize)
                    .ToListAsync(ct);
                
                return restaurantFromDb?.ToDetailResponse(
                    restaurantTablesFromDb,
                    totalTableCount,
                    query.TablePage,
                    query.TablePageSize);
            },
            new HybridCacheEntryOptions
            {
                LocalCacheExpiration = TimeSpan.FromMinutes(4),
                Expiration = TimeSpan.FromMinutes(30)
            },
            tags: [Keys.Restaurants],
            cancellationToken: cancellationToken);

        return restaurant is null
            ? Result.Failure<RestaurantDetailResponse>(RestaurantErrors.NotFound())
            : Result.Success(restaurant);
    }
}