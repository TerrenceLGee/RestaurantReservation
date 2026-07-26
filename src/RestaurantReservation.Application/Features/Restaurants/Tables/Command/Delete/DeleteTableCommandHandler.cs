using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

using RestaurantReservation.Application.Abstractions;
using RestaurantReservation.Application.Features.Constants;
using RestaurantReservation.Domain.Common;
using RestaurantReservation.Domain.Restaurants.Errors;
using RestaurantReservation.Domain.Tables.Errors;

namespace RestaurantReservation.Application.Features.Restaurants.Tables.Command.Delete;

public class DeleteTableCommandHandler(
    IApplicationDbContext context,
    HybridCache cache,
    ILogger<DeleteTableCommandHandler> logger) : IRequestHandler<DeleteTableCommand, Result>
{
    public async Task<Result> Handle(
        DeleteTableCommand command, 
        CancellationToken cancellationToken)
    {
        var restaurant = await context.Restaurants
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == command.RestaurantId, cancellationToken);

        if (restaurant is null)
        {
            logger.LogWarning("Restaurant with Id {Id} not found unable to delete table", command.RestaurantId);
            return Result.Failure(RestaurantErrors.NotFound());
        }

        var table = await context.Tables
            .FirstOrDefaultAsync(t => t.RestaurantId == command.RestaurantId
                                      && t.Id == command.TableId, cancellationToken);

        if (table is null)
        {
            logger.LogWarning("Table with Id {Id} not found in {Name}, no table deleted", 
                command.TableId, 
                restaurant.Name);
            return Result.Failure(TableErrors.TableNotFound);
        }

        if (table.IsInTableGroup)
        {
            if (!string.IsNullOrEmpty(table.TableGroupName))
            {
                var tableGroup = await context.TableGroups
                    .Include(tg => tg.Tables)
                    .FirstOrDefaultAsync(tg => tg.RestaurantId == command.RestaurantId 
                        && tg.Name.ToLower().Equals(table.TableGroupName.ToLower()), cancellationToken);

                if (tableGroup is null)
                {
                    logger.LogWarning("Table group {GName} not found in {RName} table not deleted",
                        table.TableGroupName, 
                        restaurant.Name);
                    return Result.Failure(TableErrors.TableGroupNotFound(table.TableGroupName));
                }
                
                tableGroup.RemoveTable(table);
            }
        }

        context.Tables.Remove(table);
        await context.SaveChangesAsync(cancellationToken);
        
        logger.LogInformation("A Table in {Name} was deleted, invalidating cache for key: {RKey} and {TKey} and {TGKey}",
            restaurant.Name,
            Keys.Restaurants,
            Keys.Tables,
            Keys.TableGroups);

        await cache.RemoveByTagAsync(Keys.Restaurants, cancellationToken);
        await cache.RemoveByTagAsync(Keys.Tables, cancellationToken);
        await cache.RemoveByTagAsync(Keys.TableGroups, cancellationToken);

        return Result.Success();
    }
}