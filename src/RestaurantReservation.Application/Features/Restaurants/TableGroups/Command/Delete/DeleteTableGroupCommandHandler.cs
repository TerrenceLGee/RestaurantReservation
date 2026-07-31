using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

using RestaurantReservation.Application.Abstractions;
using RestaurantReservation.Application.Features.Constants;
using RestaurantReservation.Domain.Common;
using RestaurantReservation.Domain.Restaurants.Errors;
using RestaurantReservation.Domain.Tables.Errors;

namespace RestaurantReservation.Application.Features.Restaurants.TableGroups.Command.Delete;

public sealed class DeleteTableGroupCommandHandler(
    IApplicationDbContext context,
    HybridCache cache,
    ILogger<DeleteTableGroupCommandHandler> logger) : IRequestHandler<DeleteTableGroupCommand, Result>
{
    public async Task<Result> Handle(
        DeleteTableGroupCommand command, 
        CancellationToken cancellationToken)
    {
        var restaurant = await context.Restaurants
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == command.RestaurantId, cancellationToken);

        if (restaurant is null)
        {
            logger.LogWarning("Restaurant with id {Id} not found, unable to delete table group",
                command.RestaurantId);
            return Result.Failure(RestaurantErrors.NotFound());
        }

        var tableGroup = await context.TableGroups
            .Include(tg => tg.Tables)
            .FirstOrDefaultAsync(tg => tg.Id == command.TableId && tg.RestaurantId == command.RestaurantId,
                cancellationToken);

        if (tableGroup is null)
        {
            logger.LogWarning("Table group with id {Id} not found in {RName}, unable to delete table group",
                command.TableId,
                restaurant.Name);
            return Result.Failure(TableGroupErrors.TableGroupNotFound());
        }
        
        tableGroup.RemoveTables();

        await context.SaveChangesAsync(cancellationToken);
        
        logger.LogInformation("Table group {GName} has been removed from {RName}. Invalidating cache for keys: " +
                              "{RKey}, {TGKey} and {TKey}",
            tableGroup.Name,
            restaurant.Name,
            Keys.Restaurants,
            Keys.TableGroups,
            Keys.Tables);

        await cache.RemoveByTagAsync(Keys.Restaurants, cancellationToken);
        await cache.RemoveByTagAsync(Keys.TableGroups, cancellationToken);
        await cache.RemoveByTagAsync(Keys.Tables, cancellationToken);

        return Result.Success();
    }
}