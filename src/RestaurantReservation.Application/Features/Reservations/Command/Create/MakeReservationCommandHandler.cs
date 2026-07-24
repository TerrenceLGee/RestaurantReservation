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

namespace RestaurantReservation.Application.Features.Reservations.Command.Create;

public sealed class MakeReservationCommandHandler(
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
        ReservationTable? reservationTable = null;
        List<ReservationTable> reservationTables = [];

        if (string.IsNullOrWhiteSpace(customerId))
        {
            logger.LogError("Customer {Email} is not found in the system. Reservation not made.", command.CustomerEmail);
            return Result.Failure<ReservationDetailResponse>(UserErrors.UserNotFound);
        }

        var restaurant = await context.Restaurants
            .AsNoTracking()
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
            return Result.Failure<ReservationDetailResponse>(RestaurantErrors.RestaurantClosedToday(
                restaurant.Name, 
                command.ReservationDate));
        }

        var tableToReserve = await context.Tables
            .Include(t => t.Reservations)
            .Where(t => t.SeatsAtTable >= command.NumberOfGuests && t.RestaurantId == restaurant.Id)
            .FirstOrDefaultAsync(t => !t.Reservations.Any(r => r.ScheduledReservation.ReservationDay == command.ReservationDate &&
                                                               r.ScheduledReservation.ReservationStart < command.ReservationEndTime &&
                                                               command.ReservationStartTime < r.ScheduledReservation.ReservationEnd),
                cancellationToken);

        if (tableToReserve is null)
        {
            var tableGroupToReserve = await context.TableGroups
                .Include(tg => tg.Tables)
                .ThenInclude(tbl => tbl.Reservations)
                .Where(tg => tg.Tables.Sum(t => t.SeatsAtTable) >= command.NumberOfGuests)
                .FirstOrDefaultAsync(tg => tg.Tables.All(t => !t.Reservations.Any(r => 
                    r.ScheduledReservation.ReservationDay == command.ReservationDate &&
                    r.ScheduledReservation.ReservationStart < command.ReservationEndTime &&
                    command.ReservationStartTime < r.ScheduledReservation.ReservationEnd)), cancellationToken);

            if (tableGroupToReserve is null)
            {
                logger.LogWarning("Unable to find any tables with enough capacity to accommodate the customer's party size of {Party}",
                    command.NumberOfGuests);
                return Result.Failure<ReservationDetailResponse>(ReservationErrors.UnableToSecureTablesForReservation);
            }

            foreach (var table in tableGroupToReserve.Tables)
            {
                var result = table.ReserveTable(
                    command.ReservationDate,
                    command.ReservationStartTime,
                    command.ReservationEndTime);

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
                command.ReservationDate,
                command.ReservationStartTime,
                command.ReservationEndTime);

            if (result.IsFailure)
            {
                return Result.Failure<ReservationDetailResponse>(result.Error);
            }

            reservationTable = result.Value;
        }

        var reservationResult = Reservation.MakeReservation(
            restaurant.Id,
            command.RestaurantName,
            customerId,
            command.CustomerFirstName,
            command.CustomerLastName,
            command.CustomerEmail,
            command.CustomerPhone,
            command.ReservationDate,
            command.ReservationStartTime,
            command.ReservationEndTime,
            command.NumberOfGuests);

        if (reservationResult.IsFailure)
        {
            logger.LogError("Unable to complete reservation for {CName} at {RName}",
                customerName,
                restaurant.Name);
            return Result.Failure<ReservationDetailResponse>(reservationResult.Errors);
        }

        if (reservationTable is not null)
        {
            reservationTable.ReservationId = reservationResult.Value.Id;
            reservationResult.Value.AddReservationTable(reservationTable);
        }
        else
        {
            foreach (var table in reservationTables)
            {
                table.ReservationId = reservationResult.Value.Id;
                reservationResult.Value.AddReservationTable(table);
            }
        }

        await context.Reservations.AddAsync(reservationResult.Value, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        
        logger.LogInformation("{Name} ({Email}) just made a reservation. Invalidating cache for key: {Key} and {TKey} and {TGKey}",
            customerName,
            command.CustomerEmail,
            Keys.Reservations,
            Keys.Tables,
            Keys.TableGroups);

        await cache.RemoveByTagAsync(Keys.Reservations, cancellationToken);
        await cache.RemoveByTagAsync(Keys.Tables, cancellationToken);
        await cache.RemoveByTagAsync(Keys.TableGroups, cancellationToken);

        var response = reservationResult.Value.ToDetailResponse(restaurant);

        return Result.Success(response);
    }
}