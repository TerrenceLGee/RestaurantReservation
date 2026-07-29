using RestaurantReservation.Application.Features.Reservations.Query.Responses;
using RestaurantReservation.Domain.Reservations;

namespace RestaurantReservation.Application.Features.Reservations.Query.Mappings;

public static class ReservationMappings
{
    public static ReservationDetailResponse ToDetailResponse(
        this Reservation reservation,
        string? restaurantName = null)
    {
        return new ReservationDetailResponse(
            reservation.Id,
            reservation.RestaurantId,
            restaurantName ?? "N/A",
            reservation.CustomerContactInfo.FirstName.Value,
            reservation.CustomerContactInfo.LastName.Value,
            reservation.CustomerContactInfo.EmailAddress.Value,
            reservation.CustomerContactInfo.TelephoneNumber.Value,
            reservation.ReservationInfo.Date.Value,
            reservation.ReservationInfo.StartTime.Value,
            reservation.ReservationInfo.EndTime.Value,
            reservation.ReservationCreatedAtUtc,
            reservation.ReservationUpdatedAtUtc,
            reservation.ReservationCanceledAtUtc,
            reservation.ReservationCompletedAtUtc,
            reservation.ReservationInfo.Guests.Value,
            reservation.Tables.ToList().ToDetailResponse());
    }

    public static List<ReservationTableDetailResponse> ToDetailResponse(this List<ReservationTable> tables)
    {
        var reservationTables = new List<ReservationTableDetailResponse>();

        foreach (var table in tables)
        {
            reservationTables.Add(new ReservationTableDetailResponse(table.ReservationId, table.SeatsAtTable));
        }

        return reservationTables;
    }

    public static ReservationResponse ToResponse(this Reservation reservation, string? restaurantName = null)
    {
        return new ReservationResponse(
            reservation.Id,
            reservation.RestaurantId,
            restaurantName ?? "N/A",
            reservation.CustomerContactInfo.FirstName.Value,
            reservation.CustomerContactInfo.LastName.Value,
            reservation.CustomerContactInfo.EmailAddress.Value,
            reservation.CustomerContactInfo.TelephoneNumber.Value,
            reservation.ReservationInfo.Date.Value,
            reservation.ReservationInfo.StartTime.Value,
            reservation.ReservationInfo.EndTime.Value,
            reservation.ReservationInfo.Guests.Value);
    }

    public static RescheduledReservationDetailResponse ToRescheduledDetailResponse(
        this Reservation reservation,
        DateOnly? originalDate = null,
        TimeOnly? originalStartTime = null,
        TimeOnly? originalEndTime = null)
    {
        return new RescheduledReservationDetailResponse(
            reservation.Id,
            reservation.RestaurantId,
            reservation.RestaurantName,
            reservation.CustomerContactInfo.FirstName.Value,
            reservation.CustomerContactInfo.LastName.Value,
            reservation.CustomerContactInfo.EmailAddress.Value,
            reservation.CustomerContactInfo.TelephoneNumber.Value,
            originalDate ?? new DateOnly(),
            originalStartTime ?? new TimeOnly(),
            originalEndTime ?? new TimeOnly(),
            reservation.ReservationInfo.Date.Value,
            reservation.ReservationInfo.StartTime.Value,
            reservation.ReservationInfo.EndTime.Value,
            reservation.ReservationCreatedAtUtc,
            reservation.ReservationUpdatedAtUtc,
            reservation.ReservationInfo.Guests.Value,
            reservation.Tables.ToList().ToDetailResponse());
    }
}