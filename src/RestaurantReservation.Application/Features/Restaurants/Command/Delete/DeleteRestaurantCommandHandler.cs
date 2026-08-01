using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

using RestaurantReservation.Application.Abstractions;
using RestaurantReservation.Application.Features.Constants;
using RestaurantReservation.Domain.Common;
using RestaurantReservation.Domain.Restaurants.Errors;

namespace RestaurantReservation.Application.Features.Restaurants.Command.Delete;

public sealed class DeleteRestaurantCommandHandler(
    IApplicationDbContext context,
    HybridCache cache,
    ILogger<DeleteRestaurantCommandHandler> logger) : IRequestHandler<DeleteRestaurantCommand, Result>
{
    public async Task<Result> Handle(
        DeleteRestaurantCommand command, 
        CancellationToken cancellationToken)
    {
        var restaurant = await context.Restaurants
            .FirstOrDefaultAsync(r => r.Id == command.Id, cancellationToken);

        if (restaurant is null)
        {
            logger.LogWarning("Restaurant with Id {Id} not found, nothing deleted", command.Id);
            return Result.Failure(RestaurantErrors.NotFound());
        }

        context.Restaurants.Remove(restaurant);
        await context.SaveChangesAsync(cancellationToken);
        
        logger.LogInformation("Restaurant {Name} was deleted from the system. Invalidating " +
                              "all cached entries tagged with {Tag}",
            restaurant.Name,
            Keys.Restaurants);
        await cache.RemoveByTagAsync(Keys.Restaurants, cancellationToken);

        return Result.Success();
    }
}