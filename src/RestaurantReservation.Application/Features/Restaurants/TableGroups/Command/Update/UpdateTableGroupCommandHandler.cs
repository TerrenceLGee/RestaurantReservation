using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

using RestaurantReservation.Application.Abstractions;
using RestaurantReservation.Application.Features.Constants;
using RestaurantReservation.Application.Features.Restaurants.TableGroups.Query.Responses;
using RestaurantReservation.Domain.Common;
using RestaurantReservation.Domain.Restaurants.Errors;
using RestaurantReservation.Domain.Tables;
using RestaurantReservation.Domain.Tables.Errors;

namespace RestaurantReservation.Application.Features.Restaurants.TableGroups.Command.Update;

public sealed class UpdateTableGroupCommandHandler(
    IApplicationDbContext context,
    HybridCache cache,
    ILogger<UpdateTableGroupCommandHandler> logger) : IRequestHandler<UpdateTableGroupCommand, Result>
{
    public async Task<Result> Handle(
        UpdateTableGroupCommand command, 
        CancellationToken cancellationToken)
    {
        var restaurant = await context.Restaurants
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == command.RestaurantId, cancellationToken);

        if (restaurant is null)
        {
            logger.LogWarning("Restaurant with Id {Id} not found, unable to update table group", command.RestaurantId);
            return Result.Failure(RestaurantErrors.NotFound());
        }

        if (!string.IsNullOrEmpty(command.GroupName))
        {
            var tableGroupNameAlreadyTaken = await context.TableGroups
                .AsNoTracking()
                .AnyAsync(tg => tg.RestaurantId == command.RestaurantId
                                && tg.Name.ToLower().Equals(command.GroupName.ToLower()), cancellationToken);

            if (tableGroupNameAlreadyTaken)
            {
                logger.LogWarning("There is already a table group by the name {GName} in {RName}, cannot update this " +
                                  "table group's name to already reserved table group name",
                    command.GroupName,
                    restaurant.Name);
                return Result.Failure<TableGroupDetailResponse>(
                    TableGroupErrors.TableGroupNameAlreadyTaken(command.GroupName));
            }
        }

        var tableGroup = await context.TableGroups
            .Include(tg => tg.Tables)
            .FirstOrDefaultAsync(tg => tg.Id == command.TableGroupId && tg.RestaurantId == command.RestaurantId,
                cancellationToken);

        if (tableGroup is null)
        {
            logger.LogInformation("Table group {GName} not found for {RName}, unable to update table group", 
                command.GroupName,
                restaurant.Name);
            return Result.Failure(TableGroupErrors.TableGroupNotFound());
        }

        List<Table>? tables = null;
        
        if (command.NumberOfTables.HasValue && command.NumberOfSeats.HasValue)
        {
            tables = new List<Table>();
            for (int i = 0; i < command.NumberOfTables.Value; i++)
            {
                tables.Add(restaurant.AddTable(command.NumberOfSeats.Value));
            }
        }
        
        tableGroup.Update(command.GroupName, tables);

        if (tables is not null) await context.Tables.AddRangeAsync(tables, cancellationToken);
        
        context.TableGroups.Update(tableGroup);
        await context.SaveChangesAsync(cancellationToken);
        
        logger.LogInformation("Table group with Id {GId} was updated in {RName}, invalidating cache for " +
                              "{RKey}, {TGKey} and {TKey}",
            command.TableGroupId,
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