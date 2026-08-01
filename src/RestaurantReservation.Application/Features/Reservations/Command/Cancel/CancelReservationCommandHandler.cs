using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

using RestaurantReservation.Application.Abstractions;
using RestaurantReservation.Application.Features.Constants;
using RestaurantReservation.Application.Features.Reservations.Query.Mappings;
using RestaurantReservation.Application.Features.Reservations.Query.Responses;
using RestaurantReservation.Domain.Common;
using RestaurantReservation.Domain.Reservations.Errors;
using RestaurantReservation.Domain.Reservations.ValueObjects.ReservationValueObjects;
using RestaurantReservation.Domain.Tables.Errors;
using RestaurantReservation.Domain.Users.Errors;

namespace RestaurantReservation.Application.Features.Reservations.Command.Cancel;

public sealed class CancelReservationCommandHandler(
    IApplicationDbContext context,
    HybridCache cache,
    ICurrentUser currentUser,
    ILogger<CancelReservationCommandHandler> logger) : IRequestHandler<CancelReservationCommand, Result<ReservationDetailResponse>>
{
    public async Task<Result<ReservationDetailResponse>> Handle(
        CancelReservationCommand command, 
        CancellationToken cancellationToken)
    {
        var customerId = currentUser.UserId;
        var customerEmail = currentUser.Email ?? "email";

        if (string.IsNullOrWhiteSpace(customerId))
        {
            logger.LogWarning("Customer not found in the system, no reservation was canceled");
            return Result.Failure<ReservationDetailResponse>(UserErrors.UserNotFound);
        }

        var date = new ReservationDate(command.ReservationDate);
        var start = new ReservationStart(command.ReservationStartTime);
        var end = new ReservationEnd(command.ReservationEndTime);
        var restaurantName = await context.Restaurants
            .Select(r => r.Name)
            .FirstOrDefaultAsync(cancellationToken) ?? "restaurant";

        var reservation = await context.Reservations
            .FirstOrDefaultAsync(r => r.CustomerId == customerId 
            && r.Id == command.ReservationId 
            && r.ReservationInfo.Date == date 
            && r.ReservationInfo.StartTime == start 
            && r.ReservationInfo.EndTime == end, cancellationToken);

        if (reservation is null)
        {
            logger.LogWarning("Reservation with Id {Id} not found, no reservation was canceled", 
                command.ReservationId);
            return Result.Failure<ReservationDetailResponse>(
                ReservationErrors.ReservationNotFound(customerEmail, restaurantName));
        }

        var reservationCanceledResult = reservation.CancelReservation();

        if (reservationCanceledResult.IsFailure)
        {
            logger.LogWarning("Unable to cancel reservation for {Email}: {Error}",
                customerEmail,
                reservationCanceledResult.Error.Description);
            return Result.Failure<ReservationDetailResponse>(reservationCanceledResult.Error);
        }

        var tablesFreed = await context.ReservationTables
            .Where(rt => rt.ReservationId == command.ReservationId)
            .ExecuteDeleteAsync(cancellationToken);

        if (tablesFreed <= 0)
        {
            logger.LogWarning("Unable to free the table(s) associated with reservation {Id} at restaurant {Name}",
                reservation.Id,
                restaurantName);
            return Result.Failure<ReservationDetailResponse>(TableErrors.TableNotFreed);
        }

        await context.SaveChangesAsync(cancellationToken);
        
        logger.LogInformation("Reservation for {Email} has been canceled. " +
                              "Invaliding cache for keys {Key} and {TKey}",
            customerEmail,
            Keys.Reservations,
            Keys.Tables);

        await cache.RemoveByTagAsync(Keys.Reservations, cancellationToken);
        await cache.RemoveByTagAsync(Keys.Tables, cancellationToken);

        var cancellationResponse = reservation.ToDetailResponse(restaurantName);

        return Result.Success(cancellationResponse);
    }
}