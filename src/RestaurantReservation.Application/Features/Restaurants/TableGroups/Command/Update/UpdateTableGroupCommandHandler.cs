using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

using RestaurantReservation.Application.Abstractions;
using RestaurantReservation.Domain.Common;
using RestaurantReservation.Domain.Restaurants.Errors;

namespace RestaurantReservation.Application.Features.Restaurants.TableGroups.Command.Update;

public class UpdateTableGroupCommandHandler(
    IApplicationDbContext context,
    HybridCache cache,
    ILogger<UpdateTableGroupCommandHandler> logger) : IRequestHandler<UpdateTableGroupCommand, Result>
{
    public async Task<Result> Handle(
        UpdateTableGroupCommand command, 
        CancellationToken cancellationToken)
    {
        var restaurant = await context.Restaurants
            .Include(r => r.Tables)
            .Include(r => r.TableGroups)
            .FirstOrDefaultAsync(r => r.Id == command.RestaurantId, cancellationToken);

        if (restaurant is null)
        {
            logger.LogWarning("Restaurant with Id {Id} not found, unable to update table group", command.RestaurantId);
            return Result.Failure(RestaurantErrors.NotFound());
        }

        throw new InvalidOperationException();
    }
}