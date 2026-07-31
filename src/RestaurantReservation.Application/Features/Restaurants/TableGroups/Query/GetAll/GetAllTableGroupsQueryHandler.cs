using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

using RestaurantReservation.Application.Abstractions;
using RestaurantReservation.Application.Extensions;
using RestaurantReservation.Application.Features.Constants;
using RestaurantReservation.Application.Features.Restaurants.TableGroups.Query.Mappings;
using RestaurantReservation.Application.Features.Restaurants.TableGroups.Query.Responses;
using RestaurantReservation.Domain.Common;

namespace RestaurantReservation.Application.Features.Restaurants.TableGroups.Query.GetAll;

public sealed class GetAllTableGroupsQueryHandler(
    IApplicationDbContext context,
    HybridCache cache,
    ILogger<GetAllTableGroupsQueryHandler> logger) : IRequestHandler<GetAllTableGroupsQuery, Result<PagedResult<TableGroupResponse>>>
{
    public async Task<Result<PagedResult<TableGroupResponse>>> Handle(
        GetAllTableGroupsQuery query, 
        CancellationToken cancellationToken)
    {
        var allTableGroupsQuery =
            $"restaurant:{query.RestaurantId}_tableGroups_page={query.Page}_pageSize={query.PageSize}";

        (List<TableGroupResponse> items, int totalCount) = await cache.GetOrCreateAsync(
            allTableGroupsQuery,
            async ct =>
            {
                logger.LogInformation("Cache miss for key: {Key}. Retrieving from the database.", allTableGroupsQuery);
                var tableGroupsQuery = context.TableGroups
                    .AsNoTracking()
                    .Where(tg => tg.RestaurantId == query.RestaurantId)
                    .Include(tg => tg.Restaurant)
                    .Include(tg => tg.Tables)
                    .AsQueryable();

                if (!string.IsNullOrWhiteSpace(query.Name))
                {
                    tableGroupsQuery = tableGroupsQuery.ApplySearch(query.Name);
                }

                var totalCount = await tableGroupsQuery
                    .AsNoTracking()
                    .CountAsync(ct);

                tableGroupsQuery = tableGroupsQuery.ApplySort(
                    string.IsNullOrWhiteSpace(query.SortBy) ? "Id" : query.SortBy);

                var items = await tableGroupsQuery
                    .AsNoTracking()
                    .Include(tg => tg.Tables)
                    .ApplyPagination(query.Page, query.PageSize)
                    .Select(tg => tg.ToResponse())
                    .ToListAsync(ct);

                return (items, totalCount);
            }, 
            new HybridCacheEntryOptions
            {
                LocalCacheExpiration = TimeSpan.FromMinutes(5),
                Expiration = TimeSpan.FromMinutes(30)
            },
            tags: [Keys.TableGroups],
            cancellationToken: cancellationToken);

        var pagedResult = new PagedResult<TableGroupResponse>(items, totalCount, query.Page, query.PageSize);
        return Result.Success(pagedResult);
    }
}