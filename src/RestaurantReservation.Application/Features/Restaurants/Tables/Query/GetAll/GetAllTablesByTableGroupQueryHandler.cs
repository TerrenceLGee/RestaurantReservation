using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

using RestaurantReservation.Application.Abstractions;
using RestaurantReservation.Application.Extensions;
using RestaurantReservation.Application.Features.Constants;
using RestaurantReservation.Application.Features.Restaurants.Tables.Query.Mappings;
using RestaurantReservation.Application.Features.Restaurants.Tables.Query.Responses;
using RestaurantReservation.Domain.Common;

namespace RestaurantReservation.Application.Features.Restaurants.Tables.Query.GetAll;

public sealed class GetAllTablesByTableGroupQueryHandler(
    IApplicationDbContext context,
    HybridCache cache,
    ILogger<GetAllTablesByTableGroupQueryHandler> logger) 
    : IRequestHandler<GetAllTablesByTableGroupQuery, Result<PagedResult<TableResponse>>>
{
    public async Task<Result<PagedResult<TableResponse>>> Handle(
        GetAllTablesByTableGroupQuery query, 
        CancellationToken cancellationToken)
    {
        var allTablesCacheKey = $"restaurant:{query.RestaurantId}_tableGroup:{query.TableGroupName}_tables_page={query.Page}_pageSize={query.PageSize}";

        (List<TableResponse> items, int totalCount) = await cache.GetOrCreateAsync(
            allTablesCacheKey,
            async ct =>
            {
                logger.LogInformation("Cache miss for key: {Key}. Retrieving from the database", allTablesCacheKey);
                var tablesQuery = context.Tables
                    .Where(t => t.RestaurantId == query.RestaurantId 
                    && !string.IsNullOrEmpty(t.TableGroupName) 
                    && t.TableGroupName.ToLower().Equals(query.TableGroupName.ToLower()))
                    .AsNoTracking()
                    .AsQueryable();

                var totalCount = await tablesQuery
                    .AsNoTracking()
                    .CountAsync(ct);

                tablesQuery = tablesQuery.ApplySort(
                    string.IsNullOrWhiteSpace(query.SortBy) ? "Id" : query.SortBy);

                var items = await tablesQuery
                    .AsNoTracking()
                    .ApplyPagination(query.Page, query.PageSize)
                    .Select(t => t.ToResponse())
                    .ToListAsync(ct);

                return (items, totalCount);
            },
            new HybridCacheEntryOptions
            {
                LocalCacheExpiration = TimeSpan.FromMinutes(5),
                Expiration = TimeSpan.FromMinutes(30)
            },
            tags: [Keys.Tables],
            cancellationToken: cancellationToken);

        var pagedResult = new PagedResult<TableResponse>(items, totalCount, query.Page, query.PageSize);
        return Result.Success(pagedResult);
    }
}