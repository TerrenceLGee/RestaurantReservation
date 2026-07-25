namespace RestaurantReservation.Application.Features.Reservations.Query.Responses;

public record RescheduledReservationDetailResponse(
    Guid ReservationId,
    Guid RestaurantId,
    string RestaurantName,
    string CustomerFirstName,
    string CustomerLastName,
    string CustomerEmail,
    string CustomerPhoneNumber,
    DateOnly OriginalReservationDate,
    TimeOnly OriginalReservationStartTime,
    TimeOnly OriginalReservationEndTime,
    DateOnly RescheduledReservationDate,
    TimeOnly RescheduledReservationStartTime,
    TimeOnly RescheduledReservationEndTime,
    DateTime ReservationCreatedAtUtc,
    DateTime? ReservationUpdatedAtUtc,
    int NumberOfGuests, 
    List<ReservationTableDetailResponse> TablesInReservation);