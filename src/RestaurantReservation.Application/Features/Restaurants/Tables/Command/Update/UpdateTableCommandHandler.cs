using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

using RestaurantReservation.Application.Abstractions;
using RestaurantReservation.Application.Features.Constants;
using RestaurantReservation.Domain.Common;
using RestaurantReservation.Domain.Restaurants.Errors;
using RestaurantReservation.Domain.Tables.Errors;

namespace RestaurantReservation.Application.Features.Restaurants.Tables.Command.Update;

public sealed class UpdateTableCommandHandler(
    IApplicationDbContext context,
    HybridCache cache,
    ILogger<UpdateTableCommandHandler> logger) : IRequestHandler<UpdateTableCommand, Result>
{
    public async Task<Result> Handle(
        UpdateTableCommand command, 
        CancellationToken cancellationToken)
    {
        var restaurant = await context.Restaurants
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == command.RestaurantId, cancellationToken);

        if (restaurant is null)
        {
            logger.LogWarning("Restaurant with Id {Id} not found, unable to update table", command.RestaurantId);
            return Result.Failure(RestaurantErrors.NotFound());
        }

        var table = await context.Tables
            .FirstOrDefaultAsync(t => t.RestaurantId == command.RestaurantId && t.Id == command.TableId, 
                cancellationToken);

        if (table is null)
        {
            logger.LogWarning("Table with Id {Id} not found in {Name}, unable to update table", 
                command.TableId,
                restaurant.Name);
            return Result.Failure(TableErrors.TableNotFound);
        }

        if (command.NumberOfSeats.HasValue)
        {
            table.UpdateSeating(command.NumberOfSeats.Value);
        }

        if (!string.IsNullOrEmpty(command.GroupName))
        {
            var tableGroup = await context.TableGroups
                .Include(tg => tg.Tables)
                .FirstOrDefaultAsync(
                    tg => tg.RestaurantId == command.RestaurantId &&
                          tg.Name.ToLower().Equals(command.GroupName.ToLower()), cancellationToken);
                
            if (tableGroup is null)
            {
                logger.LogWarning("Table group {Name} not found unable to finish updating table in {RName}",
                    command.GroupName,
                    restaurant.Name);
                return Result.Failure(TableErrors.TableGroupNotFound(command.GroupName));
            }
            if (!table.IsInTableGroup)
            {
                tableGroup.AddTable(table);
            }
            else
            {
                if (!string.IsNullOrEmpty(table.TableGroupName))
                {
                    if (table.TableGroupName.ToLower().Equals(command.GroupName.ToLower()))
                    {
                        logger.LogWarning("Table is already in group {GName} in {RName} unable to finish updating table",
                            command.GroupName,
                            restaurant.Name);
                        return Result.Failure(TableErrors.TableAlreadyInTableGroup(command.GroupName));
                    }

                    var originalTableGroup = await context.TableGroups
                        .Include(tg => tg.Tables)
                        .FirstOrDefaultAsync(tg => tg.RestaurantId == command.RestaurantId &&
                                                   tg.Name.ToLower().Equals(table.TableGroupName.ToLower()), cancellationToken);

                    if (originalTableGroup is null)
                    {
                        logger.LogWarning("Table group {Name} not found unable to finish updating table in {RName}",
                            table.TableGroupName,
                            restaurant.Name);
                        return Result.Failure(TableErrors.TableGroupNotFound(table.TableGroupName));
                    }
                    
                    originalTableGroup.RemoveTable(table);
                    tableGroup.AddTable(table);
                }
            }
        }

        await context.SaveChangesAsync(cancellationToken);
        
        logger.LogInformation("A Table in {Name} was updated, invalidating cache for key: {RKey} and {TKey} and {TGKey}",
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