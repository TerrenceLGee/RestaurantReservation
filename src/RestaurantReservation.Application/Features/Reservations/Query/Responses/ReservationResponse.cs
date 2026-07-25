namespace RestaurantReservation.Application.Features.Reservations.Query.Responses;

public record ReservationResponse(
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
    int NumberOfGuests);