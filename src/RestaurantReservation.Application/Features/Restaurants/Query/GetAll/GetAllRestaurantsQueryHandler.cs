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

namespace RestaurantReservation.Application.Features.Restaurants.Query.GetAll;

public sealed class GetAllRestaurantsQueryHandler(
    IApplicationDbContext context,
    HybridCache cache,
    ILogger<GetAllRestaurantsQueryHandler> logger) : IRequestHandler<GetAllRestaurantsQuery, Result<PagedResult<RestaurantResponse>>>
{
    public async Task<Result<PagedResult<RestaurantResponse>>> Handle(
        GetAllRestaurantsQuery query, 
        CancellationToken cancellationToken)
    {
        var allRestaurantsCacheKey = $"restaurants_page={query.Page}_pageSize={query.PageSize}";

        (List<RestaurantResponse> items, int totalCount) = await cache.GetOrCreateAsync(
            allRestaurantsCacheKey,
            async ct =>
            {
                logger.LogInformation("Cache miss for key: {Key}, Retrieving from the database", allRestaurantsCacheKey);
                var restaurantsQuery = context.Restaurants
                    .AsNoTracking()
                    .Include(r => r.Tables)
                    .AsSplitQuery()
                    .AsQueryable();

                if (!string.IsNullOrEmpty(query.Name))
                {
                    restaurantsQuery = restaurantsQuery.ApplySearch(query.Name);
                }

                var totalCount = await restaurantsQuery
                    .AsNoTracking()
                    .CountAsync(ct);

                restaurantsQuery = restaurantsQuery.ApplySort(
                    string.IsNullOrWhiteSpace(query.SortBy) ? "Id" : query.SortBy);

                var items = await restaurantsQuery
                    .AsNoTracking()
                    .Include(r => r.Tables)
                    .ApplyPagination(query.Page, query.PageSize)
                    .Select(r => r.ToResponse())
                    .ToListAsync(ct);

                return (items, totalCount);
            },
            new HybridCacheEntryOptions
            {
                LocalCacheExpiration = TimeSpan.FromMinutes(5),
                Expiration = TimeSpan.FromMinutes(30)
            },
            tags: [Keys.Restaurants],
            cancellationToken: cancellationToken);

        var pagedResult = new PagedResult<RestaurantResponse>(items, totalCount, query.Page, query.PageSize);
        return Result.Success(pagedResult);
    }
}