using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

using RestaurantReservation.Application.Abstractions;
using RestaurantReservation.Application.Features.Constants;
using RestaurantReservation.Application.Features.Restaurants.TableGroups.Query.Mappings;
using RestaurantReservation.Application.Features.Restaurants.TableGroups.Query.Responses;
using RestaurantReservation.Domain.Common;
using RestaurantReservation.Domain.Restaurants.Errors;
using RestaurantReservation.Domain.Tables;

namespace RestaurantReservation.Application.Features.Restaurants.TableGroups.Command.Add;

public class AddTableGroupCommandHandler(
    IApplicationDbContext context,
    HybridCache cache,
    ILogger<AddTableGroupCommandHandler> logger) : IRequestHandler<AddTableGroupCommand, Result<TableGroupDetailResponse>>
{
    public async Task<Result<TableGroupDetailResponse>> Handle(
        AddTableGroupCommand command, 
        CancellationToken cancellationToken)
    {
        var restaurant = await context.Restaurants
            .Include(r => r.Tables)
            .Include(r => r.TableGroups)
            .FirstOrDefaultAsync(r => r.Id == command.RestaurantId, cancellationToken);

        if (restaurant is null)
        {
            logger.LogWarning("Restaurant with id {Id} not found in the system unable to add table group", command.RestaurantId);
            return Result.Failure<TableGroupDetailResponse>(RestaurantErrors.NotFound());
        }

        var tableGroup = TableGroup.Create(command.RestaurantId, command.Name);

        if (command.NumberOfTables.HasValue && command.NumberOfSeats.HasValue)
        {
            var tables = new List<Table>();

            for (int i = 0; i < command.NumberOfTables.Value; i++)
            {
                tables.Add(restaurant.AddTable(command.NumberOfSeats.Value));
            }
            
            tableGroup.AddTables(tables);
        }

        await context.TableGroups.AddAsync(tableGroup, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        
        logger.LogInformation("A new table group was added to {RName}. Invalidating cache for keys: {RKey}, {TGKey}, {TKey}",
            restaurant.Name,
            Keys.Restaurants,
            Keys.TableGroups,
            Keys.Tables);

        await cache.RemoveByTagAsync(Keys.Restaurants, cancellationToken);
        await cache.RemoveByTagAsync(Keys.TableGroups, cancellationToken);
        await cache.RemoveByTagAsync(Keys.Tables, cancellationToken);

        return Result.Success(tableGroup.ToDetailResponse());
    }
}