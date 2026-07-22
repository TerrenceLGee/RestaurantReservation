namespace RestaurantReservation.Application.Features.Reservations.Notification;

public record EmailInfo(
    string RecipientName,
    string To,
    string From,
    string Subject,
    string Body);