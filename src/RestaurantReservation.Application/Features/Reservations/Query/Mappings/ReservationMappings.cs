using RestaurantReservation.Application.Features.Reservations.Query.Responses;
using RestaurantReservation.Domain.Reservations;
using RestaurantReservation.Domain.Restaurants;

namespace RestaurantReservation.Application.Features.Reservations.Query.Mappings;

public static class ReservationMappings
{
    public static ReservationDetailResponse ToDetailResponse(
        this Reservation reservation,
        Restaurant? restaurant = null)
    {
        return new ReservationDetailResponse(
            reservation.Id,
            reservation.RestaurantId,
            restaurant?.Name ?? "N/A",
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
            reservation.ReservationInfo.Guests.Value);
    }
}