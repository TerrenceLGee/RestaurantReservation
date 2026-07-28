using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

using RestaurantReservation.Application.Abstractions;
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
        var cacheKey = $"restaurant:{query.Id}";

        var restaurant = await cache.GetOrCreateAsync(
            cacheKey,
            async ct =>
            {
                logger.LogInformation("Cache miss for key: {Key}. Retrieving from database.", cacheKey);
                var restaurantFromDb = await context.Restaurants
                    .AsNoTracking()
                    .Include(r => r.TableGroups)
                    .Include(r => r.Tables)
                    .FirstOrDefaultAsync(r => r.Id == query.Id, ct);
                return restaurantFromDb?.ToDetailResponse();
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