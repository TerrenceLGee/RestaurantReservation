namespace RestaurantReservation.Application.Features.Reservations.Query.Responses;

public record ReservationDetailResponse(
    Guid ReservationId,
    Guid RestaurantId,
    string RestaurantName,
    string CustomerFirstName,
    string CustomerLastName,
    string CustomerEmail,
    string CustomerPhoneNumber,
    DateOnly ReservationDate,
    TimeOnly ReservationStartTime,
    TimeOnly ReservationEndTime,
    DateTime ReservationCreatedAtUtc,
    DateTime? ReservationUpdatedAtUtc,
    DateTime? ReservationCanceledAtUtc,
    DateTime? ReservationCompletedAtUtc,
    int NumberOfGuests);