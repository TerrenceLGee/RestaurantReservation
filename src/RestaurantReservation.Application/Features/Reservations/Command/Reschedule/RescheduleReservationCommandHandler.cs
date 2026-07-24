using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

using RestaurantReservation.Application.Abstractions;
using RestaurantReservation.Application.Features.Reservations.Query.Responses;
using RestaurantReservation.Domain.Common;
using RestaurantReservation.Domain.Reservations;
using RestaurantReservation.Domain.Reservations.Errors;
using RestaurantReservation.Domain.Restaurants.Errors;
using RestaurantReservation.Domain.Users.Errors;

namespace RestaurantReservation.Application.Features.Reservations.Command.Reschedule;

public class RescheduleReservationCommandHandler(
    IApplicationDbContext context,
    HybridCache cache,
    ICurrentUser currentUser,
    ILogger<RescheduleReservationCommandHandler> logger) : IRequestHandler<RescheduleReservationCommand, Result<ReservationDetailResponse>>
{
    public async Task<Result<ReservationDetailResponse>> Handle(
        RescheduleReservationCommand command, 
        CancellationToken cancellationToken)
    {
        var customerId = currentUser.UserId;
        var customerEmail = currentUser.Email ?? "customer";
        ReservationTable? reservationTable = null;
        List<ReservationTable> reservationTables = [];

        if (string.IsNullOrWhiteSpace(customerId))
        {
            logger.LogError("Customer that is trying to reschedule a reservation in the system is not found");
            return Result.Failure<ReservationDetailResponse>(UserErrors.UserNotFound);
        }

        var restaurant = await context.Restaurants
            .AsNoTracking()
            .Include(r => r.Schedule)
            .FirstOrDefaultAsync(r => r.Name.ToLower().Equals(command.RestaurantName.ToLower()), cancellationToken);

        if (restaurant is null)
        {
            logger.LogWarning("Restaurant {Name} not found in the system, cannot reschedule reservation",
                command.RestaurantName);
            return Result.Failure<ReservationDetailResponse>(RestaurantErrors.NotFound(command.RestaurantName));
        }

        var isOpenResult = restaurant.RestaurantIsOpen(
            command.RescheduleDate,
            command.RescheduleStartTime,
            command.RescheduleEndTime);

        if (isOpenResult.IsFailure)
        {
            logger.LogWarning("Restaurant {Name} is not opened at the time {Email} wishes to reschedule their reservation, so no reservation was rescheduled",
                command.RestaurantName,
                customerEmail);
            return Result.Failure<ReservationDetailResponse>(
                RestaurantErrors.RestaurantClosedToday(restaurant.Name, command.RescheduleDate));
        }

        var tableToReserve = await context.Tables
            .Include(t => t.Reservations)
            .Where(t => t.SeatsAtTable >= command.RescheduleNumberOfGuests && t.RestaurantId == restaurant.Id)
            .FirstOrDefaultAsync(t => !t.Reservations.Any(r => r.ReservationId != command.ReservationId 
            && r.ScheduledReservation.ReservationDay == command.RescheduleDate 
            && r.ScheduledReservation.ReservationStart < command.RescheduleEndTime 
            && command.RescheduleStartTime < r.ScheduledReservation.ReservationEnd), cancellationToken);

        if (tableToReserve is null)
        {
            var tableGroupToReserve = await context.TableGroups
                .Include(tg => tg.Tables)
                .ThenInclude(tbl => tbl.Reservations)
                .Where(tg => tg.Tables.Sum(t => t.SeatsAtTable) >= command.RescheduleNumberOfGuests)
                .FirstOrDefaultAsync(tg => tg.Tables.All(t => !t.Reservations.Any(r => r.ReservationId != command.ReservationId
                                                                         && r.ScheduledReservation.ReservationDay ==
                                                                         command.RescheduleDate
                                                                         && r.ScheduledReservation.ReservationStart <
                                                                         command.RescheduleEndTime
                                                                         && command.RescheduleStartTime <
                                                                         r.ScheduledReservation.ReservationEnd)), cancellationToken);

            if (tableGroupToReserve is null)
            {
                logger.LogWarning("Unable to find any tables with enough capacity to accommodate the customer's party size of {Party}, " +
                                  "unable to reschedule reservation",
                    command.RescheduleNumberOfGuests);
                return Result.Failure<ReservationDetailResponse>(ReservationErrors.UnableToSecureTablesForReservation);
            }

            foreach (var table in tableGroupToReserve.Tables)
            {
                var result = table.ReserveTable(
                    command.RescheduleDate,
                    command.RescheduleStartTime,
                    command.RescheduleEndTime);

                if (result.IsFailure)
                {
                    return Result.Failure<ReservationDetailResponse>(result.Error);
                }

                reservationTables.Add(result.Value);
            }
        }
        else
        {
            var result = tableToReserve.ReserveTable(
                command.RescheduleDate,
                command.RescheduleStartTime,
                command.RescheduleEndTime);

            if (result.IsFailure)
            {
                return Result.Failure<ReservationDetailResponse>(result.Error);
            }

            reservationTable = result.Value;
        }

        var reservationToReschedule = await context.Reservations
            .Include(r => r.Tables)
            .FirstOrDefaultAsync(r => r.Id == command.ReservationId, cancellationToken);

        if (reservationToReschedule is null)
        {
            logger.LogWarning("Unable to retrieve reservation {Id}, reservation for {Email} not rescheduled", 
                command.ReservationId,
                customerEmail);
            return Result.Failure<ReservationDetailResponse>(
                ReservationErrors.ReservationNotFound(customerEmail, restaurant.Name));
        }

        var rescheduledReservationResult = reservationToReschedule.RescheduleReservation(
            command.RescheduleDate,
            command.RescheduleStartTime,
            command.RescheduleEndTime,
            command.RescheduleNumberOfGuests);

        if (rescheduledReservationResult.IsFailure)
        {
            logger.LogError("Unable to reschedule reservation for {Email} at {RName}",
                customerEmail,
                restaurant.Name);
            return Result.Failure<ReservationDetailResponse>(ReservationErrors.ReservationCannotBeRescheduled);
        }
        
        throw new InvalidOperationException();
    }
}