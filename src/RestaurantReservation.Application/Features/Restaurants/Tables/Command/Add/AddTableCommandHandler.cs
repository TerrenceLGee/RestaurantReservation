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
using RestaurantReservation.Domain.Tables.Errors;

namespace RestaurantReservation.Application.Features.Restaurants.Tables.Command.Add;

public sealed class AddTableCommandHandler(
    IApplicationDbContext context,
    HybridCache cache,
    ILogger<AddTableCommandHandler> logger) : IRequestHandler<AddTableCommand, Result<RestaurantTableResponse>>
{
    public async Task<Result<RestaurantTableResponse>> Handle(
        AddTableCommand command, 
        CancellationToken cancellationToken)
    {
        var restaurant = await context.Restaurants
            .Include(r => r.Tables)
            .Include(r => r.TableGroups)
            .FirstOrDefaultAsync(r => r.Id == command.RestaurantId, cancellationToken);

        if (restaurant is null)
        {
            logger.LogWarning("Restaurant with Id {id} not found, unable to add a table to this restaurant", command.RestaurantId);
            return Result.Failure<RestaurantTableResponse>(RestaurantErrors.NotFound());
        }

        var table = restaurant.AddTable(command.NumberOfSeats);

        if (!string.IsNullOrWhiteSpace(command.TableGroup))
        {
            var isValidTableGroup = context.TableGroups.Any(tg => tg.RestaurantId == command.RestaurantId &&
                                                                  tg.Name.ToLower().Equals(command.TableGroup.ToLower()));
            if (!isValidTableGroup)
            {
                logger.LogWarning("Table group with name: {Name} not found, unable to add table", command.TableGroup);
                return Result.Failure<RestaurantTableResponse>(TableErrors.TableGroupNotFound(command.TableGroup));
            }

            restaurant.AddTableToTableGroup(table, command.TableGroup);
        }

        await context.Tables.AddAsync(table, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        
        logger.LogInformation("A Table was added to {Name}, invalidating cache for key: {RKey} and {TKey} and {TGKey}",
            restaurant.Name,
            Keys.Restaurants,
            Keys.Tables,
            Keys.TableGroups);

        await cache.RemoveByTagAsync(Keys.Restaurants, cancellationToken);
        await cache.RemoveByTagAsync(Keys.Tables, cancellationToken);
        await cache.RemoveByTagAsync(Keys.TableGroups, cancellationToken);

        return Result.Success(table.ToTableResponse());
    }
}