using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

using RestaurantReservation.Application.Abstractions;
using RestaurantReservation.Application.Features.Constants;
using RestaurantReservation.Application.Features.Reservations.Query.Mappings;
using RestaurantReservation.Application.Features.Reservations.Query.Responses;
using RestaurantReservation.Domain.Common;
using RestaurantReservation.Domain.Reservations;
using RestaurantReservation.Domain.Reservations.Errors;
using RestaurantReservation.Domain.Restaurants.Errors;
using RestaurantReservation.Domain.Tables.Errors;
using RestaurantReservation.Domain.Users.Errors;

namespace RestaurantReservation.Application.Features.Reservations.Command.Create;

public class MakeReservationCommandHandler(
    IApplicationDbContext context,
    HybridCache cache,
    ICurrentUser currentUser,
    ILogger<MakeReservationCommandHandler> logger) : IRequestHandler<MakeReservationCommand, Result<ReservationDetailResponse>>
{
    public async Task<Result<ReservationDetailResponse>> Handle(
        MakeReservationCommand command, 
        CancellationToken cancellationToken)
    {
        var customerId = currentUser.UserId;
        var customerName = currentUser.Name ?? "customer";

        if (string.IsNullOrWhiteSpace(customerId))
        {
            logger.LogError("Customer {Email} is not found in the system. Reservation not made.", command.CustomerEmail);
            return Result.Failure<ReservationDetailResponse>(UserErrors.UserNotFound);
        }

        // Change this please - don't load all of this into memory!!!
        var restaurant = await context.Restaurants
            .AsNoTracking()
            .Include(r => r.Tables)
            .ThenInclude(t => t.ScheduledReservations)
            .Include(r => r.TableGroups)
            .Include(r => r.Schedule)
            .FirstOrDefaultAsync(r => r.Name.ToLower().Equals(command.RestaurantName.ToLower()), cancellationToken);

        if (restaurant is null)
        {
            logger.LogWarning("Restaurant {Name} not found in the system, unable to make a reservation", 
                command.RestaurantName);
            return Result.Failure<ReservationDetailResponse>(RestaurantErrors.NotFound(command.RestaurantName));
        }

        var isOpenResult = restaurant.RestaurantIsOpen(
            command.ReservationDate,
            command.ReservationStartTime,
            command.ReservationEndTime);

        if (isOpenResult.IsFailure)
        {
            logger.LogWarning("Restaurant {Name} is not opened at the time {Email} wishes to schedule a reservation, so no reservation was made",
                command.RestaurantName,
                command.CustomerEmail);
        }

        var tableToReserve = restaurant.Tables
            .Where(t => t.SeatsAtTable >= command.NumberOfGuests)
            .FirstOrDefault(t => !t.ScheduledReservations.Any(r => r.ReservationDay == command.ReservationDate &&
                                                                   r.ReservationStart < command.ReservationEndTime &&
                                                                   command.ReservationStartTime < r.ReservationEnd));

        if (tableToReserve is null)
        {
            
        }

        throw new NotImplementedException();
    }
}