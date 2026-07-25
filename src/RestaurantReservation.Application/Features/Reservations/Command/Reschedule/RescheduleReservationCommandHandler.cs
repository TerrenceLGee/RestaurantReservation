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
using RestaurantReservation.Domain.Users.Errors;

namespace RestaurantReservation.Application.Features.Reservations.Command.Reschedule;

public sealed class RescheduleReservationCommandHandler(
    IApplicationDbContext context,
    HybridCache cache,
    ICurrentUser currentUser,
    ILogger<RescheduleReservationCommandHandler> logger) : IRequestHandler<RescheduleReservationCommand, Result<RescheduledReservationDetailResponse>>
{
    public async Task<Result<RescheduledReservationDetailResponse>> Handle(
        RescheduleReservationCommand command, 
        CancellationToken cancellationToken)
    {
        var customerId = currentUser.UserId;
        var customerEmail = currentUser.Email ?? "customer";
        List<ReservationTable> reservationTables = [];

        if (string.IsNullOrWhiteSpace(customerId))
        {
            logger.LogError("Customer that is trying to reschedule a reservation in the system is not found");
            return Result.Failure<RescheduledReservationDetailResponse>(UserErrors.UserNotFound);
        }
        
        var reservationToReschedule = await context.Reservations
            .Include(r => r.Tables)
            .FirstOrDefaultAsync(r => r.Id == command.ReservationId, cancellationToken);

        if (reservationToReschedule is null)
        {
            logger.LogWarning("Unable to retrieve reservation {Id}, reservation for {Email} not rescheduled", 
                command.ReservationId,
                customerEmail);
            return Result.Failure<RescheduledReservationDetailResponse>(
                ReservationErrors.ReservationNotFound(customerEmail, command.RestaurantName));
        }

        var originalReservationDate = reservationToReschedule.ReservationInfo.Date.Value;
        var originalReservationStartTime = reservationToReschedule.ReservationInfo.StartTime.Value;
        var originalReservationEndTime = reservationToReschedule.ReservationInfo.EndTime.Value;

        var restaurant = await context.Restaurants
            .AsNoTracking()
            .Include(r => r.Schedule)
            .FirstOrDefaultAsync(r => r.Name.ToLower().Equals(command.RestaurantName.ToLower()), cancellationToken);

        if (restaurant is null)
        {
            logger.LogWarning("Restaurant {Name} not found in the system, cannot reschedule reservation",
                command.RestaurantName);
            return Result.Failure<RescheduledReservationDetailResponse>(RestaurantErrors.NotFound(command.RestaurantName));
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
            return Result.Failure<RescheduledReservationDetailResponse>(
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
                return Result.Failure<RescheduledReservationDetailResponse>(ReservationErrors.UnableToSecureTablesForReservation);
            }

            foreach (var table in tableGroupToReserve.Tables)
            {
                var result = table.UpdateTableReservation(
                    reservationToReschedule.Id,
                    command.RescheduleDate,
                    command.RescheduleStartTime,
                    command.RescheduleEndTime);

                if (result.IsFailure)
                {
                    return Result.Failure<RescheduledReservationDetailResponse>(result.Error);
                }

                reservationTables = result.Value;
            }
        }
        else
        {
            var result = tableToReserve.UpdateTableReservation(
                reservationToReschedule.Id,
                command.RescheduleDate,
                command.RescheduleStartTime,
                command.RescheduleEndTime);

            if (result.IsFailure)
            {
                return Result.Failure<RescheduledReservationDetailResponse>(result.Error);
            }

            reservationTables = result.Value;
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
            return Result.Failure<RescheduledReservationDetailResponse>(ReservationErrors.ReservationCannotBeRescheduled);
        }

        var updateReservationTablesResult = reservationToReschedule.RemoveReservationTable();

        if (updateReservationTablesResult.IsFailure)
        {
            logger.LogError("Unable to reschedule reservation for {Email} at {RName}",
                customerEmail,
                restaurant.Name);
            return Result.Failure<RescheduledReservationDetailResponse>(updateReservationTablesResult.Error);
        }

        foreach (var table in reservationTables)
        {
            reservationToReschedule.AddReservationTable(table);
        }

        await context.SaveChangesAsync(cancellationToken);
        
        logger.LogError("{Email} just updated a reservation. Invaliding the cache for key: {Key} and {TKey} and {TGKey}",
            customerEmail,
            Keys.Reservations,
            Keys.Tables,
            Keys.TableGroups);

        await cache.RemoveByTagAsync(Keys.Reservations, cancellationToken);
        await cache.RemoveByTagAsync(Keys.Tables, cancellationToken);
        await cache.RemoveByTagAsync(Keys.TableGroups, cancellationToken);

        var response = reservationToReschedule.ToRescheduledDetailResponse(
            originalReservationDate,
            originalReservationStartTime,
            originalReservationEndTime);

        return Result.Success(response);
    }
}