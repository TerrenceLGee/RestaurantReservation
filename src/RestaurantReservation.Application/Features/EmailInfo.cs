namespace RestaurantReservation.Application.Features;

public record EmailInfo(
    string SenderName,
    string RecipientName,
    string To,
    string From,
    string Subject,
    string Body);