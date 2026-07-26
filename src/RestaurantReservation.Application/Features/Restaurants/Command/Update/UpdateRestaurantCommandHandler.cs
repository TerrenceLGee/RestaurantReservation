using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

using RestaurantReservation.Application.Abstractions;
using RestaurantReservation.Application.Features.Constants;
using RestaurantReservation.Domain.Common;
using RestaurantReservation.Domain.Restaurants.Errors;

namespace RestaurantReservation.Application.Features.Restaurants.Command.Update;

public class UpdateRestaurantCommandHandler(
    IApplicationDbContext context,
    HybridCache cache,
    ILogger<UpdateRestaurantCommandHandler> logger) : IRequestHandler<UpdateRestaurantCommand, Result>
{
    public async Task<Result> Handle(
        UpdateRestaurantCommand command, 
        CancellationToken cancellationToken)
    {
        var restaurant = await context.Restaurants
            .FirstOrDefaultAsync(r => r.Id == command.Id, cancellationToken);

        if (restaurant is null)
        {
            logger.LogWarning("Restaurant with id {Id} is not found in the system, nothing updated", command.Id);
            return Result.Failure(RestaurantErrors.NotFound());
        }

        restaurant.Update(command.Name, command.Schedule?.ToList());

        await context.SaveChangesAsync(cancellationToken);
        
        logger.LogInformation("Restaurant {Name} was updated in the system, " +
                              "Invalidating all cache entries tagged with {Tag}",
            restaurant.Name,
            Keys.Restaurants);

        await cache.RemoveByTagAsync(Keys.Restaurants, cancellationToken);

        return Result.Success();
    }
}