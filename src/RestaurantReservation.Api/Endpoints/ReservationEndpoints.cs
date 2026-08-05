using MediatR;

using RestaurantReservation.Api.Extensions;
using RestaurantReservation.Application.Features.Reservations.Command.Cancel;
using RestaurantReservation.Application.Features.Reservations.Command.Complete;
using RestaurantReservation.Application.Features.Reservations.Command.Reschedule;
using RestaurantReservation.Application.Features.Reservations.Command.Schedule;
using RestaurantReservation.Application.Features.Reservations.Query.Get;
using RestaurantReservation.Application.Features.Reservations.Query.GetAll;
using RestaurantReservation.Domain.Reservations.Events;
using RestaurantReservation.Domain.Users;

namespace RestaurantReservation.Api.Endpoints;

public static class ReservationEndpoints
{
    public static void MapReservationEndpoints(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup(Constants.Reservation.BaseUri)
            .WithTags(Constants.Reservation.Tag)
            .WithDescription("Schedule, Reschedule, Complete or Cancel a Reservation")
            .RequireAuthorization();

        api.MapPost(Constants.Reservation.ScheduleReservation, ScheduleReservation)
            .WithName("ScheduleReservation")
            .WithSummary("Schedule a reservation with one of our restaurants");

        api.MapPut(Constants.Reservation.RescheduleReservation, RescheduleReservation)
            .WithName("RescheduleReservation")
            .WithSummary("Reschedule a reservation");

        api.MapPut(Constants.Reservation.CancelReservation, CancelReservation)
            .WithName("CancelReservation")
            .WithSummary("Cancel a reservation");

        api.MapPut(Constants.Reservation.CompleteReservation, CompleteReservation)
            .WithName("CompleteReservation")
            .WithSummary("Complete your reservation");

        api.MapGet(Constants.Reservation.GetAll, GetAll)
            .WithName("GetAllReservations")
            .WithSummary("Get all your reservations");

        api.MapGet(Constants.Reservation.GetAllByRestaurant, GetAllByRestaurant)
            .WithName("GetAllRestaurantReservations")
            .WithSummary("Get all reservations tied to a restaurant in the system (Admins Only)")
            .RequireAuthorization(policy =>
                policy.RequireRole(Roles.Admin));

        api.MapGet(Constants.Reservation.Get, Get)
            .WithName("GetReservation")
            .WithSummary("Get one of your individual reservations");
    }

    private static async Task<IResult> ScheduleReservation(
        ScheduleReservationCommand command,
        IMediator sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(command, cancellationToken);

        if (result.IsFailure) return result.ToProblemDetails();

        await sender.Publish(new ReservationScheduledEvent(
            result.Value.ReservationId,
            result.Value.CustomerFirstName,
            result.Value.CustomerLastName,
            result.Value.CustomerEmail,
            result.Value.RestaurantName,
            result.Value.ReservationDate,
            result.Value.ReservationStartTime,
            result.Value.ReservationEndTime,
            result.Value.NumberOfGuests),
            cancellationToken);

        return TypedResults.Created($"/api/reservations/{result.Value.ReservationId}", result.Value);
    }

    private static async Task<IResult> RescheduleReservation(
        RescheduleReservationCommand command,
        IMediator sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(command, cancellationToken);

        if (result.IsFailure) return result.ToProblemDetails();

        await sender.Publish(new ReservationRescheduledEvent(
                result.Value.ReservationId,
                result.Value.CustomerFirstName,
                result.Value.CustomerLastName,
                result.Value.CustomerEmail,
                result.Value.RestaurantName,
                result.Value.OriginalReservationDate,
                result.Value.OriginalReservationStartTime,
                result.Value.OriginalReservationEndTime,
                result.Value.RescheduledReservationDate,
                result.Value.RescheduledReservationStartTime,
                result.Value.RescheduledReservationEndTime,
                result.Value.OriginalNumberOfGuests),
            cancellationToken);

        return TypedResults.Ok(result.Value);
    }

    private static async Task<IResult> CancelReservation(
        CancelReservationCommand command,
        IMediator sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(command, cancellationToken);

        if (result.IsFailure) return result.ToProblemDetails();

        await sender.Publish(new ReservationCanceledEvent(
                result.Value.ReservationId,
                result.Value.CustomerFirstName,
                result.Value.CustomerLastName,
                result.Value.CustomerEmail,
                result.Value.RestaurantName,
                result.Value.ReservationDate,
                result.Value.ReservationStartTime,
                result.Value.ReservationEndTime,
                result.Value.ReservationCanceledAtUtc!.Value), 
            cancellationToken);

        return TypedResults.Ok(result.Value);
    }

    private static async Task<IResult> CompleteReservation(
        CompleteReservationCommand command,
        IMediator sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(command, cancellationToken);

        if (result.IsFailure) return result.ToProblemDetails();

        await sender.Publish(new ReservationCompletedEvent(
                result.Value.ReservationId,
                result.Value.CustomerFirstName,
                result.Value.CustomerLastName,
                result.Value.CustomerEmail,
                result.Value.RestaurantName,
                result.Value.ReservationDate,
                result.Value.ReservationStartTime,
                result.Value.ReservationEndTime,
                result.Value.ReservationCompletedAtUtc!.Value),
            cancellationToken);

        return TypedResults.Ok(result.Value);
    }

    private static async Task<IResult> GetAll(
        int? page,
        int? pageSize,
        string? sortBy,
        string? restaurantName,
        string? status,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var query = new GetAllReservationsQuery(
            page ?? 1,
            pageSize ?? 10,
            sortBy,
            restaurantName,
            status);

        var result = await sender.Send(query, cancellationToken);

        return result.IsSuccess
            ? TypedResults.Ok(result.Value)
            : result.ToProblemDetails();
    }

    private static async Task<IResult> GetAllByRestaurant(
        Guid restaurantId,
        int? page,
        int? pageSize,
        string? sortBy,
        string? status,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var query = new GetAllRestaurantReservationsQuery(
            restaurantId,
            page ?? 1,
            pageSize ?? 10,
            sortBy,
            status);

        var result = await sender.Send(query, cancellationToken);

        return result.IsSuccess
            ? TypedResults.Ok(result.Value)
            : result.ToProblemDetails();
    }

    private static async Task<IResult> Get(
        Guid reservationId,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var query = new GetReservationQuery(reservationId);
        var result = await sender.Send(query, cancellationToken);

        return result.IsSuccess
            ? TypedResults.Ok(result.Value)
            : result.ToProblemDetails();
    }
}