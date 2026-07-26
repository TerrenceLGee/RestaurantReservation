using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

using RestaurantReservation.Application.Abstractions;
using RestaurantReservation.Application.Features.Constants;
using RestaurantReservation.Application.Features.Restaurants.Query.Mappings;
using RestaurantReservation.Application.Features.Restaurants.Query.Response;
using RestaurantReservation.Domain.Common;
using RestaurantReservation.Domain.Restaurants;
using RestaurantReservation.Domain.Restaurants.Errors;
using RestaurantReservation.Domain.Tables;

namespace RestaurantReservation.Application.Features.Restaurants.Command.Add;

public sealed class AddRestaurantCommandHandler(
    IApplicationDbContext context,
    HybridCache cache,
    ILogger<AddRestaurantCommandHandler> logger) : IRequestHandler<AddRestaurantCommand, Result<RestaurantDetailResponse>>
{
    public async Task<Result<RestaurantDetailResponse>> Handle(
        AddRestaurantCommand command, 
        CancellationToken cancellationToken)
    {
        var alreadyExists = await context.Restaurants
            .AnyAsync(r => r.Name.ToLower().Equals(command.Name.ToLower()), cancellationToken);

        if (alreadyExists)
        {
            logger.LogWarning("A restaurant named {Name} already exists in the system and cannot be added",
                command.Name);
            return Result.Failure<RestaurantDetailResponse>(RestaurantErrors.AlreadyExists(command.Name));
        }

        var restaurant = Restaurant.Create(command.Name);
        
        restaurant.SetSchedule([.. command.Schedule]);

        foreach (var info in command.TableInfo)
        {
            var tables = new List<Table>();
            for (int i = 0; i < info.NumberOfTables; i++)
            {
                tables.Add(restaurant.AddTable(info.NumberOfSeats));
            }

            if (!string.IsNullOrEmpty(info.GroupName))
            {
                restaurant.AddTableGroup(info.GroupName, tables);
            }
        }

        await context.Restaurants.AddAsync(restaurant, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        
        logger.LogInformation("A new restaurant was added: '{Name}'. Invalidating all cache entries " +
                              "tagged with {Tag}",
            command.Name,
            Keys.Restaurants);
        await cache.RemoveByTagAsync(Keys.Restaurants, cancellationToken);

        return Result.Success(restaurant.ToDetailResponse());
    }
}